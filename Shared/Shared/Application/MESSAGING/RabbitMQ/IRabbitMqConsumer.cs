namespace SharedKernel.Infrastructure.Messaging.RabbitMQ;

/// <summary>
/// Marker interface for a module's RabbitMQ consumer background service.
/// </summary>
public interface IRabbitMqConsumer
{
    Task StopAsync(CancellationToken cancellationToken);
}
