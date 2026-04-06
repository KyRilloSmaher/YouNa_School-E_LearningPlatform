namespace SharedKernel.Infrastructure.Messaging.RabbitMQ;

/// <summary>
/// Configuration options for RabbitMQ connection and topology
/// </summary>
public sealed class RabbitMqOptions
{
    public const string SectionName = "RabbitMQ";

    /// <summary>
    /// RabbitMQ host name or IP address
    /// </summary>
    public string HostName { get; init; } = "localhost";

    /// <summary>
    /// RabbitMQ port (default: 5672)
    /// </summary>
    public int Port { get; init; } = 5672;

    /// <summary>
    /// Username for authentication
    /// </summary>
    public string UserName { get; init; } = "guest";

    /// <summary>
    /// Password for authentication
    /// </summary>
    public string Password { get; init; } = "guest";

    /// <summary>
    /// Virtual host (default: "/")
    /// </summary>
    public string VirtualHost { get; init; } = "/";

    /// <summary>
    /// Enable automatic connection recovery
    /// </summary>
    public bool AutomaticRecoveryEnabled { get; init; } = true;

    /// <summary>
    /// Network recovery interval in seconds
    /// </summary>
    public int NetworkRecoveryIntervalSeconds { get; init; } = 10;

    /// <summary>
    /// Connection timeout in seconds
    /// </summary>
    public int RequestedConnectionTimeoutSeconds { get; init; } = 30;

    /// <summary>
    /// Heartbeat interval in seconds (0 = disabled)
    /// </summary>
    public ushort RequestedHeartbeatSeconds { get; init; } = 60;

    /// <summary>
    /// Prefetch count for consumer (QoS)
    /// </summary>
    public ushort PrefetchCount { get; init; } = 1;

    /// <summary>
    /// Maximum retry attempts for failed messages
    /// </summary>
    public int MaxRetryAttempts { get; init; } = 3;

    /// <summary>
    /// Retry delay in seconds
    /// </summary>
    public int RetryDelaySeconds { get; init; } = 5;

    /// <summary>
    /// Dead letter exchange name
    /// </summary>
    public string DeadLetterExchange { get; init; } = "dlx.events";
}