using RabbitMQ.Client;

namespace YouNaSchool.Wallet.Infrastructure.Messaging.RabbitMQ
{
    internal static class RabbitMqTopology
    {
        public static async Task InitializeAsync(
            IConnection connection,
            CancellationToken cancellationToken = default)
        {
            var channel = await connection.CreateChannelAsync();

            // Exchange for Wallet events
            await channel.ExchangeDeclareAsync(
                exchange: "wallet.events",
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false,
                arguments: null,
                cancellationToken: cancellationToken
            );

            await channel.CloseAsync();
        }
    }
}