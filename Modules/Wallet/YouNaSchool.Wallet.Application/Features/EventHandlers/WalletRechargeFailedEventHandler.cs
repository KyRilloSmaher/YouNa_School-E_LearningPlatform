using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel.Application.ClockandUserContext;
using SharedKernel.Application.Messaging.Outbox;
using SharedKernel.Application.OUTBOX_PATTERN;
using System.Text.Json;
using YouNaSchool.Wallet.Application.Abstractions.Messaging;
using YouNaSchool.Wallet.Domain.Events;

public sealed class WalletRechargeFailedEventHandler
    : INotificationHandler<WalletRechargeFailedEvent>
{
    private readonly ILogger<WalletRechargeFailedEventHandler> _logger;
    private readonly IOutBoxMessageRepository _outbox;
    private readonly ISystemClock _clock;

    public WalletRechargeFailedEventHandler(
        ILogger<WalletRechargeFailedEventHandler> logger,
        IOutBoxMessageRepository outbox,
        ISystemClock clock)
    {
        _logger = logger;
        _outbox = outbox;
        _clock = clock;
    }

    public async Task Handle(WalletRechargeFailedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogError(
            "Recharge {RechargeId} failed for wallet {WalletId}",
            notification.RechargeId,
            notification.WalletId);

        //await _outbox.AddAsync(new OutboxMessage
        //{
        //    Id = Guid.NewGuid(),
        //    Type = nameof(WalletRechargeFailedEvent),
        //    Payload = JsonSerializer.Serialize(notification),
        //    OccurredOn = _clock.UtcNow
        //}, cancellationToken);
    }
}