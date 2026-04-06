using Shared.Application.IntegrationEvent;
using Shared.Application.IntegrationEvents;
using SharedKernel.Application.Messaging.Outbox;
using System.Text.Json;

namespace SharedKernel.Infrastructure.Messaging.Outbox;

/// <summary>
/// Implements IIntegrationEventPublisher by writing events to the outbox table.
/// Must be called within an active database transaction so the event and the
/// domain change are committed atomically.
///
/// Usage in a command handler:
///   await _publisher.PublishAsync(new UserCreatedIntegrationEvent(...));
///   await _unitOfWork.CommitAsync(); // ← outbox row is committed here
/// </summary>
public sealed class OutboxIntegrationEventPublisher : IIntegrationEventPublisher
{
    private readonly IOutBoxMessageRepository _repository;
    private readonly string _exchangeName;
    private readonly string _routingKeyPrefix;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    /// <param name="repository">Outbox repository scoped to this module's DbContext</param>
    /// <param name="exchangeName">The module's RabbitMQ exchange (e.g. "users.events")</param>
    /// <param name="routingKeyPrefix">Prefix for routing keys (e.g. "users")</param>
    public OutboxIntegrationEventPublisher(
        IOutBoxMessageRepository repository,
        string exchangeName,
        string routingKeyPrefix)
    {
        _repository = repository;
        _exchangeName = exchangeName;
        _routingKeyPrefix = routingKeyPrefix;
    }

    public async Task PublishAsync<TEvent>(
        TEvent @event,
        CancellationToken cancellationToken = default)
        where TEvent : IIntegrationEvent
    {
        var eventType = @event.GetType();
        var typeName = eventType.Name; // e.g. "UserCreatedIntegrationEvent"
        var payload = JsonSerializer.Serialize(@event, eventType, JsonOptions);

        // Routing key: "users.UserCreatedIntegrationEvent"
        var routingKey = $"{_routingKeyPrefix}.{typeName}";

        var message = OutboxMessage.Create(
            type: typeName,
            payload: payload,
            exchange: _exchangeName,
            routingKey: routingKey);

        await _repository.AddAsync(message, cancellationToken);
    }
}