namespace YouNaSchool.Wallet.Infrastructure.Messaging.RabbitMQ
{
    public sealed class RabbitMqOptions
    {
        public string HostName { get; init; } = null!;
        public int Port { get; init; } = 5672;
        public string UserName { get; init; } = null!;
        public string Password { get; init; } = null!;
        public string VirtualHost { get; init; } = "/";

        // Exchange and Queue settings
        public string ExchangeName { get; set; } = "wallet.events";
        public string ExchangeType { get; set; } = "topic";
        public string QueueName { get; set; } = "identity.wallet";
        public bool Durable { get; set; } = true;
        public bool AutoDelete { get; set; } = false;
        public string RoutingKey { get; init; } = default!;
    }
}