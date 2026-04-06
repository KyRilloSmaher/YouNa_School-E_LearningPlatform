using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace SharedKernel.Infrastructure.Messaging.RabbitMQ;

/// <summary>
/// Publisher interface for sending messages to RabbitMQ
/// </summary>
public interface IRabbitMqPublisher
{
    Task PublishAsync<TMessage>(
        string exchange,
        string routingKey,
        TMessage message,
        CancellationToken cancellationToken = default) where TMessage : class;

    Task PublishAsync(
        string exchange,
        string routingKey,
        string messageType,
        string payload,
        CancellationToken cancellationToken = default);
}

internal sealed class RabbitMqPublisher : IRabbitMqPublisher, IAsyncDisposable
{
    private readonly IRabbitMqConnectionFactory _connectionFactory;
    private readonly ILogger<RabbitMqPublisher> _logger;
    private readonly SemaphoreSlim _channelLock = new(1, 1);
    private readonly JsonSerializerOptions _jsonOptions;
    private IChannel? _channel;
    private bool _disposed;

    public RabbitMqPublisher(
        IRabbitMqConnectionFactory connectionFactory,
        ILogger<RabbitMqPublisher> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    private async Task<IChannel> CreateChannelAsync()
    {
        var connection = await _connectionFactory.CreateConnectionAsync();
        var channel = await connection.CreateChannelAsync();

        _logger.LogDebug("RabbitMQ publisher channel created");
        return channel;
    }

    private async Task<IChannel> GetOrCreateChannelAsync(CancellationToken cancellationToken)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(RabbitMqPublisher));

        if (_channel is { IsOpen: true })
            return _channel;

        await _channelLock.WaitAsync(cancellationToken);
        try
        {
            if (_channel is { IsOpen: true })
                return _channel;

            if (_channel is not null)
            {
                try
                {
                    await _channel.CloseAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(ex, "Ignoring RabbitMQ publisher channel close failure during recreation");
                }

                _channel.Dispose();
                _channel = null;
            }

            _channel = await CreateChannelAsync();
            return _channel;
        }
        finally
        {
            _channelLock.Release();
        }
    }

    public async Task PublishAsync<TMessage>(
        string exchange,
        string routingKey,
        TMessage message,
        CancellationToken cancellationToken = default) where TMessage : class
    {
        var messageType = message.GetType().Name;
        var payload = JsonSerializer.Serialize(message, _jsonOptions);

        await PublishAsync(exchange, routingKey, messageType, payload, cancellationToken);
    }

    public async Task PublishAsync(
        string exchange,
        string routingKey,
        string messageType,
        string payload,
        CancellationToken cancellationToken = default)
    {
        var properties = new BasicProperties
        {
            Persistent = true,
            ContentType = "application/json",
            ContentEncoding = "utf-8",
            Type = messageType,
            MessageId = Guid.NewGuid().ToString(),
            Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds()),
            AppId = "YouNaSchool",
            DeliveryMode = DeliveryModes.Persistent
        };

        var body = Encoding.UTF8.GetBytes(payload);

        try
        {
            var channel = await GetOrCreateChannelAsync(cancellationToken);

            await channel.BasicPublishAsync(
                exchange: exchange,
                routingKey: routingKey,
                mandatory: false,
                basicProperties: properties,
                body: body,
                cancellationToken: cancellationToken);

            _logger.LogDebug(
                "Published message {MessageType} to exchange {Exchange} with routing key {RoutingKey}",
                messageType,
                exchange,
                routingKey);
        }
        catch (Exception ex)
        {
            await ResetChannelAsync();
            _logger.LogError(
                ex,
                "Failed to publish message {MessageType} to exchange {Exchange}",
                messageType,
                exchange);
            throw;
        }
    }

    private async Task ResetChannelAsync()
    {
        await _channelLock.WaitAsync();
        try
        {
            if (_channel is null)
                return;

            try
            {
                if (_channel.IsOpen)
                    await _channel.CloseAsync();
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Ignoring RabbitMQ publisher channel close failure during reset");
            }

            _channel.Dispose();
            _channel = null;
        }
        finally
        {
            _channelLock.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;

        _disposed = true;

        await _channelLock.WaitAsync();
        try
        {
            if (_channel is not null)
            {
                try
                {
                    await _channel.CloseAsync();
                    _channel.Dispose();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error disposing RabbitMQ publisher channel");
                }
            }
        }
        finally
        {
            _channelLock.Release();
            _channelLock.Dispose();
        }

        GC.SuppressFinalize(this);
    }
}
