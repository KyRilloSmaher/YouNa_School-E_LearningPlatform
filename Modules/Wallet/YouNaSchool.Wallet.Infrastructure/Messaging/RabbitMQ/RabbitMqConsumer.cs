using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using YouNaSchool.Wallet.Application.Abstractions.Messaging;
using YouNaSchool.Wallet.Infrastructure.Messaging.RabbitMQ;

internal sealed class RabbitMqConsumer : IRabbitMqConsumer
{
    private readonly RabbitMqOptions _options;
    private readonly IIntegrationEventHandler _handler;
    private readonly ILogger<RabbitMqConsumer> _logger;

    private IConnection? _connection;
    private IChannel? _channel;

    public RabbitMqConsumer(RabbitMqOptions options,IIntegrationEventHandler handler,ILogger<RabbitMqConsumer> logger)
    {
        _options = options;
        _handler = handler;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _options.HostName,
            Port = _options.Port,
            UserName = _options.UserName,
            Password = _options.Password,
            VirtualHost = _options.VirtualHost,
            AutomaticRecoveryEnabled = true,
            NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
        };

        _connection = await factory.CreateConnectionAsync(stoppingToken);
        _channel = await _connection.CreateChannelAsync();

        // Exchange (publisher owns it, but safe to declare)
        await _channel.ExchangeDeclareAsync(
            exchange: _options.ExchangeName,
            type: _options.ExchangeType,
            durable: _options.Durable,
            autoDelete: _options.AutoDelete,
            cancellationToken: stoppingToken);

        // Queue (consumer owns it)
        await _channel.QueueDeclareAsync(
            queue: _options.QueueName,
            durable: _options.Durable,
            exclusive: false,
            autoDelete: false,
            cancellationToken: stoppingToken);

        await _channel.QueueBindAsync(
            queue: _options.QueueName,
            exchange: _options.ExchangeName,
            routingKey: _options.RoutingKey,
            cancellationToken: stoppingToken);

        // Fair dispatch
        await _channel.BasicQosAsync(
            prefetchSize: 0,
            prefetchCount: 1,
            global: false,
            cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (sender, ea) =>
        {
            try
            {
                _logger.LogInformation(
                    "Received message {MessageId} ({Type})",
                    ea.BasicProperties.MessageId,
                    ea.BasicProperties.Type);

                await _handler.HandleAsync(
                    ea.BasicProperties.Type,
                    ea.Body,
                    stoppingToken);

                await _channel.BasicAckAsync(
                    deliveryTag: ea.DeliveryTag,
                    multiple: false,
                    cancellationToken: stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error processing message {MessageId}",
                    ea.BasicProperties.MessageId);

                // Requeue = false → DLQ (if configured)
                await _channel.BasicNackAsync(
                    deliveryTag: ea.DeliveryTag,
                    multiple: false,
                    requeue: false,
                    cancellationToken: stoppingToken);
            }
        };

        await _channel.BasicConsumeAsync(
            queue: _options.QueueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken);

        _logger.LogInformation(
            "RabbitMQ consumer started. Queue: {Queue}",
            _options.ExchangeName);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping RabbitMQ consumer");

        if (_channel is not null)
            await _channel.CloseAsync(cancellationToken);

        if (_connection is not null)
            await _connection.CloseAsync(cancellationToken);
    }
}