using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace SharedKernel.Infrastructure.Messaging.RabbitMQ;

/// <summary>
/// Background service that consumes messages from RabbitMQ
/// </summary>
public sealed class RabbitMqConsumer : BackgroundService , IRabbitMqConsumer
{
    private readonly IRabbitMqConnectionFactory _connectionFactory;
    private readonly IIntegrationEventDispatcher _dispatcher;
    private readonly ModuleMessagingConfiguration _configuration;
    private readonly RabbitMqOptions _options;
    private readonly ILogger<RabbitMqConsumer> _logger;

    private IConnection? _connection;
    private IChannel? _channel;

    public RabbitMqConsumer(
        IRabbitMqConnectionFactory connectionFactory,
        IIntegrationEventDispatcher dispatcher,
        ModuleMessagingConfiguration configuration,
        IOptions<RabbitMqOptions> options,
        ILogger<RabbitMqConsumer> logger)
    {
        _connectionFactory = connectionFactory;
        _dispatcher = dispatcher;
        _configuration = configuration;
        _options = options.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "Starting RabbitMQ consumer for module: {ModuleName}",
            _configuration.ModuleName);

        try
        {
            _connection = await _connectionFactory.CreateConnectionAsync(stoppingToken);
            _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

            // Set QoS (prefetch count)
            await _channel.BasicQosAsync(
                prefetchSize: 0,
                prefetchCount: _options.PrefetchCount,
                global: false,
                cancellationToken: stoppingToken);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (sender, ea) =>
            {
                await ProcessMessageAsync(ea, stoppingToken);
            };

            await _channel.BasicConsumeAsync(
                queue: _configuration.QueueName,
                autoAck: false,
                consumer: consumer,
                cancellationToken: stoppingToken);

            _logger.LogInformation(
                "RabbitMQ consumer started successfully for queue: {QueueName}",
                _configuration.QueueName);

            // Keep the consumer running
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("RabbitMQ consumer is stopping");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Fatal error in RabbitMQ consumer for module: {ModuleName}",
                _configuration.ModuleName);
            throw;
        }
    }

    private async Task ProcessMessageAsync(
        BasicDeliverEventArgs ea,
        CancellationToken cancellationToken)
    {
        var messageId = ea.BasicProperties.MessageId ?? "unknown";
        var messageType = ea.BasicProperties.Type ?? "unknown";

        try
        {
            _logger.LogDebug(
                "Processing message {MessageId} of type {MessageType}",
                messageId,
                messageType);

            var payload = Encoding.UTF8.GetString(ea.Body.ToArray());

            await _dispatcher.DispatchAsync(messageType, payload, cancellationToken);

            // Acknowledge successful processing
            if (_channel is not null)
            {
                await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false, cancellationToken);

                _logger.LogDebug(
                    "Successfully processed message {MessageId}",
                    messageId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error processing message {MessageId} of type {MessageType}",
                messageId,
                messageType);

            await HandleFailedMessageAsync(ea, ex, cancellationToken);
        }
    }

    private async Task HandleFailedMessageAsync(
        BasicDeliverEventArgs ea,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (_channel is null)
            return;

        var retryCount = GetRetryCount(ea.BasicProperties);
        var maxRetries = _options.MaxRetryAttempts;

        if (retryCount < maxRetries)
        {
            // Requeue with delay using dead letter exchange pattern
            _logger.LogWarning(
                "Requeueing message {MessageId} (Retry {RetryCount}/{MaxRetries})",
                ea.BasicProperties.MessageId,
                retryCount + 1,
                maxRetries);

            await _channel.BasicNackAsync(
                deliveryTag: ea.DeliveryTag,
                multiple: false,
                requeue: false, // Send to DLQ
                cancellationToken: cancellationToken);
        }
        else
        {
            // Max retries exceeded, send to dead letter queue
            _logger.LogError(
                "Max retries exceeded for message {MessageId}. Sending to dead letter queue",
                ea.BasicProperties.MessageId);

            await _channel.BasicNackAsync(
                deliveryTag: ea.DeliveryTag,
                multiple: false,
                requeue: false,
                cancellationToken: cancellationToken);
        }
    }

    private static int GetRetryCount(IReadOnlyBasicProperties properties)
    {
        if (properties.Headers is not null &&
            properties.Headers.TryGetValue("x-retry-count", out var value))
        {
            return value is int count ? count : 0;
        }

        return 0;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Stopping RabbitMQ consumer for module: {ModuleName}",
            _configuration.ModuleName);

        if (_channel is not null)
        {
            await _channel.CloseAsync(cancellationToken);
        }

        if (_connection is not null)
        {
            await _connection.CloseAsync(cancellationToken);
        }

        await base.StopAsync(cancellationToken);
    }

}