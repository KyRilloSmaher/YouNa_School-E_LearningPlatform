using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel.Application.ClockandUserContext;
using SharedKernel.Application.Messaging.Outbox;
using SharedKernel.Application.OUTBOX_PATTERN;
using System.Text.Json;
using YouNaSchool.Wallet.Application.Abstractions.Messaging;
using YouNaSchool.Wallet.Domain.Events;

public sealed class WalletRechargeCompletedEventHandler
    : INotificationHandler<WalletRechargeCompletedEvent>
{
    private readonly ILogger<WalletRechargeCompletedEventHandler> _logger;
    private readonly IOutBoxMessageRepository _outbox;
    private readonly ISystemClock _clock;

    public WalletRechargeCompletedEventHandler(
        ILogger<WalletRechargeCompletedEventHandler> logger,
        IOutBoxMessageRepository outbox,
        ISystemClock clock)
    {
        _logger = logger;
        _outbox = outbox;
        _clock = clock;
    }

    public async Task Handle(WalletRechargeCompletedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Recharge {RechargeId} completed for wallet {WalletId}",
            notification.RechargeId,
            notification.WalletId);

        //await _outbox.AddAsync(new OutboxMessage
        //{
        //    Id = Guid.NewGuid(),
        //    Type = nameof(WalletRechargeCompletedEvent),
        //    Payload = JsonSerializer.Serialize(notification),
        //    OccurredOn = _clock.UtcNow
        //}, cancellationToken);
    }
}