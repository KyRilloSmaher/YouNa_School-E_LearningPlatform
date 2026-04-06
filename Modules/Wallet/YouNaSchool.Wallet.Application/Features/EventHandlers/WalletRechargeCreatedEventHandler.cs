using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel.Application.ClockandUserContext;
using SharedKernel.Application.Messaging.Outbox;
using SharedKernel.Application.OUTBOX_PATTERN;
using System.Text.Json;
using YouNaSchool.Wallet.Application.Abstractions.Messaging;
using YouNaSchool.Wallet.Domain.Events;

public sealed class WalletRechargeCreatedEventHandler
    : INotificationHandler<WalletRechargeCreatedEvent>
{
    private readonly ILogger<WalletRechargeCreatedEventHandler> _logger;
    private readonly IOutBoxMessageRepository _outbox;
    private readonly ISystemClock _clock;

    public WalletRechargeCreatedEventHandler(
        ILogger<WalletRechargeCreatedEventHandler> logger,
        IOutBoxMessageRepository outbox,
        ISystemClock clock)
    {
        _logger = logger;
        _outbox = outbox;
        _clock = clock;
    }

    public async Task Handle(WalletRechargeCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Recharge {RechargeId} created for wallet {WalletId} with amount {Amount}",
            notification.RechargeId,
            notification.WalletId,
            notification.Amount);

        //await _outbox.AddAsync(new OutboxMessage
        //{
        //    Id = Guid.NewGuid(),
        //    Type = nameof(WalletRechargeCreatedEvent),
        //    Payload = JsonSerializer.Serialize(notification),
        //    OccurredOn = _clock.UtcNow
        //}, cancellationToken);
        
    }
}