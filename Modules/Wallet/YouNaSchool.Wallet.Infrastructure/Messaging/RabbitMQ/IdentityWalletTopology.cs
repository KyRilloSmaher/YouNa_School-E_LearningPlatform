using RabbitMQ.Client;

internal static class IdentityWalletTopology
{
    public static async Task InitializeAsync(
        IConnection connection,
        CancellationToken cancellationToken = default)
    {
        var channel = await connection.CreateChannelAsync();

        await channel.ExchangeDeclareAsync(
            "wallet.events",
            ExchangeType.Topic,
            durable: true,
            autoDelete: false
        );

        await channel.QueueDeclareAsync(
            queue: "identity.wallet",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        await channel.QueueBindAsync(
            queue: "identity.wallet",
            exchange: "wallet.events",
            routingKey: "wallet.*"
        );

        await channel.CloseAsync();
    }
}