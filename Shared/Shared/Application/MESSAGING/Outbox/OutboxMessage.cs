/// <summary>
/// Represents a reliably-stored message that will be published to the message broker.
/// Saved in the same transaction as the domain change that triggered the event.
/// </summary>
public sealed class OutboxMessage
{
    public Guid Id { get; private set; }

    /// <summary>
    /// The CLR type name of the integration event (used for deserialization)
    /// </summary>
    public string Type { get; private set; } = null!;

    /// <summary>
    /// JSON-serialized event payload
    /// </summary>
    public string Payload { get; private set; } = null!;

    /// <summary>
    /// RabbitMQ exchange to publish to
    /// </summary>
    public string Exchange { get; private set; } = null!;

    /// <summary>
    /// RabbitMQ routing key
    /// </summary>
    public string RoutingKey { get; private set; } = null!;

    /// <summary>
    /// UTC time the message was created (and the event occurred)
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// UTC time the message was successfully published; null = not yet published
    /// </summary>
    public DateTime? PublishedAt { get; private set; }

    /// <summary>
    /// Number of delivery attempts made
    /// </summary>
    public int RetryCount { get; private set; }

    /// <summary>
    /// Error message from last failed attempt
    /// </summary>
    public string? Error { get; private set; }

    private OutboxMessage() { } // EF Core

    public static OutboxMessage Create(
        string type,
        string payload,
        string exchange,
        string routingKey)
    {
        return new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = type,
            Payload = payload,
            Exchange = exchange,
            RoutingKey = routingKey,
            CreatedAt = DateTime.UtcNow,
            RetryCount = 0
        };
    }

    public void MarkAsPublished()
    {
        PublishedAt = DateTime.UtcNow;
        Error = null;
    }

    public void MarkAsFailed(string error)
    {
        RetryCount++;
        Error = error;
    }
}