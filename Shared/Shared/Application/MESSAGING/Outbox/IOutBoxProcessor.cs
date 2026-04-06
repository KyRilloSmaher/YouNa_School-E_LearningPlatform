namespace SharedKernel.Application.Messaging.Outbox
{
    /// <summary>
    /// Processes pending outbox messages and publishes them to RabbitMQ
    /// </summary>
    public interface IOutBoxProcessor
    {
        Task ProcessAsync(CancellationToken cancellationToken = default);
    }
}