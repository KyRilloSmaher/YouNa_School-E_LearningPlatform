using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shared.Application.IntegrationEvents;
using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json;

namespace SharedKernel.Infrastructure.Messaging.RabbitMQ;

/// <summary>
/// Dispatches integration events to their registered handlers
/// </summary>
public interface IIntegrationEventDispatcher
{
    Task DispatchAsync(
        string messageType,
        string payload,
        CancellationToken cancellationToken = default);
}

internal sealed class IntegrationEventDispatcher : IIntegrationEventDispatcher
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<IntegrationEventDispatcher> _logger;
    private readonly ConcurrentDictionary<string, Type> _eventTypeCache = new();
    private readonly JsonSerializerOptions _jsonOptions;

    public IntegrationEventDispatcher(
        IServiceProvider serviceProvider,
        ILogger<IntegrationEventDispatcher> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task DispatchAsync(
        string messageType,
        string payload,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // 1. Resolve event type from cache or discover it
            var eventType = _eventTypeCache.GetOrAdd(messageType, ResolveEventType);

            if (eventType is null)
            {
                _logger.LogWarning(
                    "Event type {MessageType} not found in any loaded assembly",
                    messageType);
                return;
            }

            // 2. Deserialize the payload
            var @event = JsonSerializer.Deserialize(payload, eventType, _jsonOptions);

            if (@event is null)
            {
                _logger.LogWarning(
                    "Failed to deserialize event {MessageType}",
                    messageType);
                return;
            }

            // 3. Resolve and invoke handler
            await InvokeHandlerAsync(eventType, @event, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error dispatching integration event {MessageType}",
                messageType);
            throw;
        }
    }

    private Type? ResolveEventType(string messageType)
    {
        _logger.LogDebug("Resolving event type for: {MessageType}", messageType);

        var eventType = AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(a => !a.IsDynamic)
            .SelectMany(a =>
            {
                try
                {
                    return a.GetTypes();
                }
                catch
                {
                    return Array.Empty<Type>();
                }
            })
            .FirstOrDefault(t =>
                t.Name == messageType &&
                typeof(IIntegrationEvent).IsAssignableFrom(t));

        if (eventType is not null)
        {
            _logger.LogDebug(
                "Resolved event type {MessageType} to {FullName}",
                messageType,
                eventType.FullName);
        }

        return eventType;
    }

    private async Task InvokeHandlerAsync(
        Type eventType,
        object @event,
        CancellationToken cancellationToken)
    {
        var handlerType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);

        using var scope = _serviceProvider.CreateScope();

        var handlers = scope.ServiceProvider.GetServices(handlerType).ToList();

        if (!handlers.Any())
        {
            _logger.LogWarning(
                "No handler registered for event type {EventType}",
                eventType.Name);
            return;
        }

        _logger.LogDebug(
            "Found {HandlerCount} handler(s) for event {EventType}",
            handlers.Count,
            eventType.Name);

        foreach (var handler in handlers)
        {
            var handleMethod = handlerType.GetMethod(
                nameof(IIntegrationEventHandler<IIntegrationEvent>.HandleAsync),
                BindingFlags.Public | BindingFlags.Instance);

            if (handleMethod is null)
                continue;

            try
            {
                await (Task)handleMethod.Invoke(handler, new[] { @event, cancellationToken })!;

                _logger.LogDebug(
                    "Successfully handled event {EventType} with handler {HandlerType}",
                    eventType.Name,
                    handler.GetType().Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error handling event {EventType} with handler {HandlerType}",
                    eventType.Name,
                    handler.GetType().Name);
                throw;
            }
        }
    }
}