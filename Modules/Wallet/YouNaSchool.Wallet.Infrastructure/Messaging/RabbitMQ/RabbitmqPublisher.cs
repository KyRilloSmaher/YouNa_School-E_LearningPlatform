using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Text;
using YouNaSchool.Wallet.Application.Abstractions.Messaging;

namespace YouNaSchool.Wallet.Infrastructure.Messaging.RabbitMQ
{
    internal sealed class RabbitMqPublisher : IRabbitMqPublisher, IAsyncDisposable
    {
        private readonly Lazy<Task<IConnection>> _connectionTask;
        private readonly Lazy<Task<IChannel>> _channelTask;
        private readonly ILogger<RabbitMqPublisher> _logger;

        public RabbitMqPublisher(
            RabbitMqOptions options,
            ILogger<RabbitMqPublisher> logger)
        {
            _logger = logger;

            _connectionTask = new Lazy<Task<IConnection>>(
                async () =>
                {
                    var factory = new ConnectionFactory
                    {
                        HostName = options.HostName,
                        Port = options.Port,
                        UserName = options.UserName,
                        Password = options.Password,
                        VirtualHost = options.VirtualHost,
                        AutomaticRecoveryEnabled = true,
                        NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
                    };

                    _logger.LogInformation(
                        "Connecting to RabbitMQ at {Host}", options.HostName);

                    return await factory.CreateConnectionAsync();
                },
                LazyThreadSafetyMode.ExecutionAndPublication);

            _channelTask = new Lazy<Task<IChannel>>(
                async () =>
                {
                    var connection = await _connectionTask.Value;
                    var channel = await connection.CreateChannelAsync();

                    //await channel.ConfirmSelectAsync();

                    _logger.LogInformation("RabbitMQ channel created");

                    return channel;
                },
                LazyThreadSafetyMode.ExecutionAndPublication);
        }

        public async Task PublishAsync(
            string exchange,
            string routingKey,
            string messageType,
            string payload,
            CancellationToken cancellationToken = default)
        {
            var channel = await _channelTask.Value;

            var body = Encoding.UTF8.GetBytes(payload);

            var properties = new BasicProperties
            {
                Persistent = true,
                ContentType = "application/json",
                Type = messageType,
                MessageId = Guid.NewGuid().ToString(),
                Timestamp = new AmqpTimestamp(
                    DateTimeOffset.UtcNow.ToUnixTimeSeconds())
            };

            _logger.LogDebug(
                "Publishing message to {Exchange} with routing key {RoutingKey}",
                exchange, routingKey);

            await channel.BasicPublishAsync(
                exchange: exchange,
                routingKey: routingKey,
                mandatory: false,
                basicProperties: properties,
                body: body,
                cancellationToken: cancellationToken);

            //await channel.WaitForConfirmsAsync(cancellationToken);
        }

        public async ValueTask DisposeAsync()
        {
            if (_channelTask.IsValueCreated)
            {
                var channel = await _channelTask.Value;
                await channel.CloseAsync();
            }

            if (_connectionTask.IsValueCreated)
            {
                var connection = await _connectionTask.Value;
                await connection.CloseAsync();
            }

            GC.SuppressFinalize(this);
        }
    }
}