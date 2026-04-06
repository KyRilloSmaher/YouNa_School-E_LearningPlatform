using Microsoft.Extensions.Logging;
using SharedKernel.Infrastructure.Messaging.RabbitMQ;
using YounaSchool.Authuntication.Application.Abstractions.Messaging;

namespace YounaSchool.Authuntication.Infrastructure.Messaging;

internal sealed class IntegrationEventPublisher : IIntegrationEventPublisher
{
    private const string ExchangeName = "auth.events";
    private readonly IRabbitMqPublisher _publisher;
    private readonly ILogger<IntegrationEventPublisher> _logger;

    public IntegrationEventPublisher(
        IRabbitMqPublisher publisher,
        ILogger<IntegrationEventPublisher> logger)
    {
        _publisher = publisher;
        _logger = logger;
    }

    public Task PublishAsync<T>(T integrationEvent, CancellationToken cancellationToken = default)
        where T : class
    {
        var routingKey = GetRoutingKey(typeof(T).Name);
        return _publisher.PublishAsync(ExchangeName, routingKey, integrationEvent, cancellationToken);
    }

    private static string GetRoutingKey(string eventTypeName)
    {
        var baseName = eventTypeName.EndsWith("IntegrationEvent")
            ? eventTypeName[..^16]
            : eventTypeName;

        if (baseName.EndsWith("Event"))
            baseName = baseName[..^5];

        return string.Concat(
            baseName.Select((c, i) =>
                i > 0 && char.IsUpper(c)
                    ? "." + char.ToLowerInvariant(c)
                    : char.ToLowerInvariant(c).ToString()));
    }
}
