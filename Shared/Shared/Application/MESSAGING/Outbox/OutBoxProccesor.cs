//using Microsoft.Extensions.Logging;
//using SharedKernel.Application.Messaging.Outbox;
//using SharedKernel.Application.OUTBOX_PATTERN;
//using SharedKernel.Application.UNIT_OF_WORK;
//using SharedKernel.Infrastructure.Messaging.RabbitMQ;
//using System.Text.Json;

//namespace SharedKernel.Infrastructure.Messaging.Outbox;

///// <summary>
///// Processes outbox messages and publishes them to RabbitMQ
///// </summary>
//internal sealed class OutboxProcessor : IOutBoxProcessor
//{
//    private readonly IOutBoxMessageRepository _repository;
//    private readonly IRabbitMqPublisher _publisher;
//    private readonly IUnitOfWork _unitOfWork;
//    private readonly ILogger<OutboxProcessor> _logger;
//    private readonly string _exchangeName;

//    public OutboxProcessor(
//        IOutBoxMessageRepository repository,
//        IRabbitMqPublisher publisher,
//        IUnitOfWork unitOfWork,
//        string exchangeName,
//        ILogger<OutboxProcessor> logger)
//    {
//        _repository = repository;
//        _publisher = publisher;
//        _unitOfWork = unitOfWork;
//        _exchangeName = exchangeName;
//        _logger = logger;
//    }

//    public async Task ProcessAsync(CancellationToken cancellationToken = default)
//    {
//        var messages = await _repository.GetUnprocessedAsync(20, cancellationToken);

//        if (!messages.Any())
//            return;

//        _logger.LogInformation(
//            "Processing {Count} outbox messages",
//            messages.Count);

//        var successCount = 0;
//        var failureCount = 0;

//        foreach (var message in messages)
//        {
//            try
//            {
//                var routingKey = ResolveRoutingKey(message.Type);

//                _logger.LogDebug(
//                    "Publishing outbox message {MessageId} of type {MessageType}",
//                    message.Id,
//                    message.Type);

//                await _publisher.PublishAsync(
//                    exchange: _exchangeName,
//                    routingKey: routingKey,
//                    messageType: message.Type,
//                    payload: message.Payload,
//                    cancellationToken: cancellationToken);

//                await _repository.MarkAsProcessedAsync(message.Id, cancellationToken);
//                successCount++;

//                _logger.LogDebug(
//                    "Successfully published outbox message {MessageId}",
//                    message.Id);
//            }
//            catch (Exception ex)
//            {
//                failureCount++;
//                var errorMessage = $"{ex.GetType().Name}: {ex.Message}";

//                _logger.LogError(
//                    ex,
//                    "Failed to publish outbox message {MessageId} of type {MessageType}",
//                    message.Id,
//                    message.Type);

//                await _repository.MarkAsFailedAsync(
//                    message.Id,
//                    errorMessage,
//                    cancellationToken);
//            }
//        }

//        // Save all changes in one transaction
//        await _unitOfWork.SaveChangesAsync(cancellationToken);

//        _logger.LogInformation(
//            "Outbox processing completed: {SuccessCount} succeeded, {FailureCount} failed",
//            successCount,
//            failureCount);
//    }

//    private static string ResolveRoutingKey(string messageType)
//    {
//        // Remove "IntegrationEvent" suffix if present
//        var baseName = messageType.EndsWith("IntegrationEvent")
//            ? messageType[..^16]
//            : messageType;

//        // Remove "Event" suffix if present
//        if (baseName.EndsWith("Event"))
//        {
//            baseName = baseName[..^5];
//        }

//        // Convert PascalCase to dot.separated.lowercase
//        var routing = string.Concat(
//            baseName.Select((c, i) =>
//                i > 0 && char.IsUpper(c)
//                    ? "." + char.ToLowerInvariant(c)
//                    : char.ToLowerInvariant(c).ToString()
//            ));

//        return routing;
//    }
//}