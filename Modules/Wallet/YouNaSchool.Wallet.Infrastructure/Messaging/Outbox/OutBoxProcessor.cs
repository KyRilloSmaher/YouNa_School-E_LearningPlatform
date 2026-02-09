using Microsoft.Extensions.Logging;
using SharedKernel.Application.UNIT_OF_WORK;
using YouNaSchool.Wallet.Application.Abstractions.Messaging;
using YouNaSchool.Wallet.Infrastructure.Messaging.RabbitMQ;

namespace YouNaSchool.Wallet.Infrastructure.Messaging.Outbox
{
    internal sealed class OutBoxProcessor : IOutBoxProcessor
    {
        private readonly IOutBoxMessageRepository _outboxRepository;
        private readonly IRabbitMqPublisher _publisher;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<OutBoxProcessor> _logger;

        private const string WalletExchange = "wallet.events";

        public OutBoxProcessor(
            IOutBoxMessageRepository outboxRepository,
            IRabbitMqPublisher publisher,
            IUnitOfWork unitOfWork,
            ILogger<OutBoxProcessor> logger)
        {
            _outboxRepository = outboxRepository;
            _publisher = publisher;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task ProcessAsync(CancellationToken cancellationToken = default)
        {
            var messages = await _outboxRepository
                .GetUnprocessedAsync(20, cancellationToken);

            if (!messages.Any())
                return;

            foreach (var message in messages)
            {
                try
                {
                    var routingKey = ResolveRoutingKey(message.Type);

                    _logger.LogInformation(
                        "Publishing Outbox message {MessageId} to {RoutingKey}",
                        message.Id,
                        routingKey);

                    await _publisher.PublishAsync(
                        exchange: WalletExchange,
                        routingKey: routingKey,
                        messageType: message.Type,
                        messageBody: message.Payload,
                        cancellationToken: cancellationToken);

                    message.ProcessedOn = DateTime.UtcNow;
                    message.Error = null;
                }
                catch (Exception ex)
                {
                    message.Error = ex.Message;

                    _logger.LogError(
                        ex,
                        "Failed to process Outbox message {MessageId}",
                        message.Id);
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        private static string ResolveRoutingKey(string messageType)
        {
            // Explicit mapping = maintainable + safe
            return messageType switch
            {
                "WalletCreatedEvent" => "wallet.created",
                "WalletCreditedEvent" => "wallet.credited",
                "WalletDebitedEvent" => "wallet.debited",
                "WalletDeactivatedEvent" => "wallet.deactivated",
                "WalletRechargeCompletedEvent" => "wallet.recharge.completed",
                _ => throw new InvalidOperationException(
                        $"Unknown outbox message type: {messageType}")
            };
        }
    }
}