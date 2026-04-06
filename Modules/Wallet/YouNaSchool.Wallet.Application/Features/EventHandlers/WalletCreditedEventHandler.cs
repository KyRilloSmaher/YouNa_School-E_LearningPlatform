using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel.Application.ClockandUserContext;
using SharedKernel.Application.Messaging.Outbox;
using System.Text.Json;
using YouNaSchool.Wallet.Application.Abstractions.OtherModules;
using YouNaSchool.Wallet.Application.IntegrationEvents;
using YouNaSchool.Wallet.Domain.Events;

public sealed class WalletCreditedEventHandler: INotificationHandler<WalletCreditedEvent>
{
    private readonly ILogger<WalletCreditedEventHandler> _logger;
    private readonly IOutBoxMessageRepository _outbox;
    private readonly ISystemClock _clock;
    private readonly IAuthUserProvider _userEmailQuery;

    public WalletCreditedEventHandler(
        ILogger<WalletCreditedEventHandler> logger,
        IOutBoxMessageRepository outbox,
        ISystemClock clock,
        IAuthUserProvider userEmailQuery)
    {
        _logger = logger;
        _outbox = outbox;
        _clock = clock;
        _userEmailQuery = userEmailQuery;
    }

    public async Task Handle(WalletCreditedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Wallet {WalletId} credited with {Amount}",
            notification.WalletId,
            notification.Amount);

        var userId = Guid.Parse(notification.StudentId);

        var email = await _userEmailQuery.GetUserEmailAsync(userId, cancellationToken);

        // 1️⃣ Create Integration Event
        var integrationEvent = new WalletRechargedIntegrationEvent(
            UserId: userId,
            Amount: notification.Amount,
            RechargedAt: _clock.UtcNow,
            phone: null,
            Email: email
        );

        // 2️⃣ Serialize
        var payload = JsonSerializer.Serialize(integrationEvent);

        // 3️⃣ Save to Outbox
        var outboxMessage = OutboxMessage.Create(
            type: nameof(WalletRechargedIntegrationEvent),
            payload: payload,
            exchange: "notifications.exchange",
            routingKey: "notifications.wallet.recharged"
        );

        await _outbox.AddAsync(outboxMessage, cancellationToken);
    }
}