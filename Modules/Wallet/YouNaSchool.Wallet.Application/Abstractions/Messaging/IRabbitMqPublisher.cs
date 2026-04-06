namespace YouNaSchool.Wallet.Application.Abstractions.Messaging
{
    public interface IRabbitMqPublisher
    {
        Task PublishAsync(
            string exchange,
            string routingKey,
            string messageType,
            string messageBody,
            CancellationToken cancellationToken = default);
    }
}