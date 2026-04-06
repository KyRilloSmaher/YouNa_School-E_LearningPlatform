namespace SharedKernel.Infrastructure.Messaging.RabbitMQ;

/// <summary>
/// Configuration for a module's RabbitMQ topology (exchanges, queues, bindings)
/// </summary>
public sealed class ModuleMessagingConfiguration
{
    /// <summary>
    /// Module name (e.g., "users", "wallet", "auth")
    /// </summary>
    public string ModuleName { get; init; } = null!;

    /// <summary>
    /// Exchange name for publishing events (e.g., "users.events")
    /// </summary>
    public string ExchangeName { get; init; } = null!;

    /// <summary>
    /// Exchange type (topic, direct, fanout, headers)
    /// </summary>
    public string ExchangeType { get; init; } = "topic";

    /// <summary>
    /// Queue name for consuming events (e.g., "wallet.users.queue")
    /// </summary>
    public string QueueName { get; init; } = null!;

    /// <summary>
    /// Routing key patterns for binding queue to exchanges
    /// Examples: ["users.*", "auth.user.created"]
    /// </summary>
    public List<RoutingKeyBinding> RoutingKeyBindings { get; init; } = new();

    /// <summary>
    /// Whether the exchange should be durable
    /// </summary>
    public bool Durable { get; init; } = true;

    /// <summary>
    /// Whether the exchange should auto-delete when unused
    /// </summary>
    public bool AutoDelete { get; init; } = false;

    /// <summary>
    /// Enable dead letter queue for failed messages
    /// </summary>
    public bool EnableDeadLetterQueue { get; init; } = true;

    /// <summary>
    /// Dead letter queue name
    /// </summary>
    public string DeadLetterQueueName => $"{QueueName}.dlq";
}

/// <summary>
/// Represents a binding between a queue and an exchange with a routing key
/// </summary>
public sealed class RoutingKeyBinding
{
    /// <summary>
    /// The exchange to bind from
    /// </summary>
    public string ExchangeName { get; init; } = null!;

    /// <summary>
    /// The routing key pattern (supports wildcards: * and #)
    /// </summary>
    public string RoutingKey { get; init; } = null!;

    public RoutingKeyBinding(string exchangeName, string routingKey)
    {
        ExchangeName = exchangeName;
        RoutingKey = routingKey;
    }
}