using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel.Application.ClockandUserContext;
using SharedKernel.Application.Messaging.Outbox;
using SharedKernel.Application.OUTBOX_PATTERN;
using System.Text.Json;
using YouNaSchool.Wallet.Application.Abstractions.Messaging;
using YouNaSchool.Wallet.Domain.Events;

namespace YouNaSchool.Wallet.Application.Features.EventHandlers
{
    public sealed class WalletCreatedEventHandler
        : INotificationHandler<WalletCreatedEvent>
    {
        private readonly ILogger<WalletCreatedEventHandler> _logger;
        private readonly IOutBoxMessageRepository _outbox;
        private readonly ISystemClock _clock;

        public WalletCreatedEventHandler(
            ILogger<WalletCreatedEventHandler> logger,
            IOutBoxMessageRepository outbox,
            ISystemClock clock)
        {
            _logger = logger;
            _outbox = outbox;
            _clock = clock;
        }

        public async Task Handle(WalletCreatedEvent notification, CancellationToken cancellationToken)
        {
            if (notification is null)
            {
                _logger.LogWarning("WalletCreatedEvent received null.");
                return;
            }

            _logger.LogInformation(
                "Wallet {WalletId} created for student {StudentId}",
                notification.WalletId,
                notification.StudentId);
            // Adding Actions Later
        }
    }
}