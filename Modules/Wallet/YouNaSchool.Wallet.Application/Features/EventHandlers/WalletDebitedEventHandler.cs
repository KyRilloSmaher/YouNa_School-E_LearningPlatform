using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel.Application.ClockandUserContext;
using System.Text.Json;
using YouNaSchool.Wallet.Domain.Events;
using YouNaSchool.Wallet.Application.Abstractions.OtherModules;
using SharedKernel.Application.Messaging.Outbox;
using YouNaSchool.Wallet.Application.IntegrationEvents;

public sealed class WalletDebitedEventHandler
    : INotificationHandler<WalletDebitedEvent>
{
    private readonly ILogger<WalletDebitedEventHandler> _logger;
    private readonly IOutBoxMessageRepository _outbox;
    private readonly ISystemClock _clock;
    private readonly IAuthUserProvider _userEmailQuery;

    public WalletDebitedEventHandler(
        ILogger<WalletDebitedEventHandler> logger,
        IOutBoxMessageRepository outbox,
        ISystemClock clock,
        IAuthUserProvider userEmailQuery)
    {
        _logger = logger;
        _outbox = outbox;
        _clock = clock;
        _userEmailQuery = userEmailQuery;
    }

    public async Task Handle(WalletDebitedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Wallet {WalletId} debited with {Amount}. Reference {ReferenceId}",
            notification.WalletId,
            notification.Amount,
            notification.ReferenceId);

        //  Get user email from Auth module
        var userId = Guid.Parse(notification.StudentId);

        var email = await _userEmailQuery.GetUserEmailAsync(userId, cancellationToken);

        if (string.IsNullOrEmpty(email))
        {
            _logger.LogWarning("No email found for user {StudentId}", notification.StudentId);
            return;
        }

        // Create Integration Event
        var integrationEvent = new LecturePaidIntegrationEvent(
            UserId: userId,
            LectureId: notification.ReferenceId,
            Amount: notification.Amount,
            PaidAt: _clock.UtcNow,
            phone: null, // if you have it
            Email: email
        );

        // Serialize
        var payload = JsonSerializer.Serialize(integrationEvent);

        //Save to Outbox
        var outboxMessage = OutboxMessage.Create(
            type: nameof(LecturePaidIntegrationEvent),
            payload: payload,
            exchange: "notifications.exchange",
            routingKey: "notifications.lecture.paid"
        );

        await _outbox.AddAsync(outboxMessage, cancellationToken);
    }
}