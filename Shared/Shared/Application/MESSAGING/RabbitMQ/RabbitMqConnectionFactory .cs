using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace SharedKernel.Infrastructure.Messaging.RabbitMQ;

/// <summary>
/// Factory for creating and managing RabbitMQ connections with automatic recovery
/// </summary>
public interface IRabbitMqConnectionFactory
{
    Task<IConnection> CreateConnectionAsync(CancellationToken cancellationToken = default);
    IConnection GetOrCreateConnection();
}

internal sealed class RabbitMqConnectionFactory : IRabbitMqConnectionFactory, IAsyncDisposable
{
    private readonly RabbitMqOptions _options;
    private readonly ILogger<RabbitMqConnectionFactory> _logger;
    private readonly SemaphoreSlim _connectionLock = new(1, 1);
    private IConnection? _connection;
    private bool _disposed;

    public RabbitMqConnectionFactory(
        IOptions<RabbitMqOptions> options,
        ILogger<RabbitMqConnectionFactory> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task<IConnection> CreateConnectionAsync(CancellationToken cancellationToken = default)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(RabbitMqConnectionFactory));

        await _connectionLock.WaitAsync(cancellationToken);
        try
        {
            if (_connection is not null && _connection.IsOpen)
                return _connection;

            _logger.LogInformation(
                "Creating RabbitMQ connection to {HostName}:{Port}",
                _options.HostName,
                _options.Port);

            var factory = new ConnectionFactory
            {
                HostName = _options.HostName,
                Port = _options.Port,
                UserName = _options.UserName,
                Password = _options.Password,
                VirtualHost = _options.VirtualHost,
                AutomaticRecoveryEnabled = _options.AutomaticRecoveryEnabled,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(_options.NetworkRecoveryIntervalSeconds),
                RequestedConnectionTimeout = TimeSpan.FromSeconds(_options.RequestedConnectionTimeoutSeconds),
                RequestedHeartbeat = TimeSpan.FromSeconds(_options.RequestedHeartbeatSeconds)
            };

            _connection = await factory.CreateConnectionAsync(cancellationToken);

            RegisterConnectionEvents(_connection);

            _logger.LogInformation("RabbitMQ connection established successfully");

            return _connection;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to create RabbitMQ connection to {HostName}:{Port}",
                _options.HostName,
                _options.Port);
            throw;
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    public IConnection GetOrCreateConnection()
    {
        if (_connection is not null && _connection.IsOpen)
            return _connection;

        return CreateConnectionAsync().GetAwaiter().GetResult();
    }

    private void RegisterConnectionEvents(IConnection connection)
    {
        connection.ConnectionShutdownAsync += async (sender, args) =>
        {
            if (!args.Initiator.Equals(ShutdownInitiator.Application))
            {
                _logger.LogWarning(
                    "RabbitMQ connection shut down: {ReplyText}",
                    args.ReplyText);
            }
            await Task.CompletedTask;
        };

        connection.CallbackExceptionAsync += async (sender, args) =>
        {
            _logger.LogError(
                args.Exception,
                "RabbitMQ connection callback exception: {Detail}",
                args.Detail);
            await Task.CompletedTask;
        };

        connection.ConnectionBlockedAsync += async (sender, args) =>
        {
            _logger.LogWarning(
                "RabbitMQ connection blocked: {Reason}",
                args.Reason);
            await Task.CompletedTask;
        };

        connection.ConnectionUnblockedAsync += async (sender, args) =>
        {
            _logger.LogInformation("RabbitMQ connection unblocked");
            await Task.CompletedTask;
        };
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;

        _disposed = true;

        if (_connection is not null)
        {
            try
            {
                await _connection.CloseAsync();
                _connection.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disposing RabbitMQ connection");
            }
        }

        _connectionLock.Dispose();
        GC.SuppressFinalize(this);
    }
}