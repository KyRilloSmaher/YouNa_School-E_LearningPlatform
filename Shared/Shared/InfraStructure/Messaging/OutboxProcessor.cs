using Microsoft.Extensions.Logging;
using SharedKernel.Application.Messaging.Outbox;
using SharedKernel.Application.UNIT_OF_WORK;
using SharedKernel.Infrastructure.Messaging.RabbitMQ;

namespace SharedKernel.Infrastructure.Messaging.Outbox;

/// <summary>
/// Reads pending outbox messages and publishes them to RabbitMQ.
/// Invoked periodically by a Hangfire recurring job.
/// </summary>
public sealed class OutboxProcessor : IOutBoxProcessor
{
    private readonly IOutBoxMessageRepository _repository;
    private readonly IRabbitMqPublisher _publisher;
    private readonly IUnitOfWork _unitOfWork;
    private readonly string _exchangeName;
    private readonly ILogger<OutboxProcessor> _logger;

    private const int BatchSize = 50;

    public OutboxProcessor(
        IOutBoxMessageRepository repository,
        IRabbitMqPublisher publisher,
        IUnitOfWork unitOfWork,
        string exchangeName,
        ILogger<OutboxProcessor> logger)
    {
        _repository = repository;
        _publisher = publisher;
        _unitOfWork = unitOfWork;
        _exchangeName = exchangeName;
        _logger = logger;
    }

    public async Task ProcessAsync(CancellationToken cancellationToken = default)
    {
        var messages = await _repository.GetPendingAsync(BatchSize, cancellationToken);

        if (!messages.Any())
        {
            _logger.LogDebug("No pending outbox messages for exchange {Exchange}", _exchangeName);
            return;
        }

        _logger.LogInformation(
            "Processing {Count} pending outbox message(s) for exchange {Exchange}",
            messages.Count,
            _exchangeName);

        foreach (var message in messages)
        {
            await ProcessMessageAsync(message, cancellationToken);
        }
    }

    private async Task ProcessMessageAsync(
        OutboxMessage message,
        CancellationToken cancellationToken)
    {
        try
        {
            await _publisher.PublishAsync(
                exchange: message.Exchange,
                routingKey: message.RoutingKey,
                messageType: message.Type,
                payload: message.Payload,
                cancellationToken: cancellationToken);

            message.MarkAsPublished();

            _logger.LogDebug(
                "Published outbox message {MessageId} ({MessageType})",
                message.Id,
                message.Type);
        }
        catch (Exception ex)
        {
            message.MarkAsFailed(ex.Message);

            _logger.LogError(
                ex,
                "Failed to publish outbox message {MessageId} ({MessageType}). Retry #{RetryCount}",
                message.Id,
                message.Type,
                message.RetryCount);
        }
        finally
        {
            await _repository.UpdateAsync(message, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}