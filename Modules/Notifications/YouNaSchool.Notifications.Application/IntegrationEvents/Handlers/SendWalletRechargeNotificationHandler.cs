
using YouNaSchool.Notifications.Domain.Enums;
using YouNaSchool.Notifications.Domain.Interfaces.Repositories;
using YouNaSchool.Notifications.Domain.Models;
namespace YouNaSchool.Notifications.Application.IntegrationEvents.Handlers
{


    public class SendWalletRechargeNotificationHandler : IIntegrationEventHandler<WalletRechargedIntegrationEvent>
    {
        private readonly INotificationRepository _notificationRepo;
        private readonly INotificationPreferenceRepository _preferenceRepo;

        public SendWalletRechargeNotificationHandler(
            INotificationRepository notificationRepo,
            INotificationPreferenceRepository preferenceRepo)
        {
            _notificationRepo = notificationRepo;
            _preferenceRepo = preferenceRepo;
        }

        public async Task HandleAsync(WalletRechargedIntegrationEvent evt, CancellationToken ct)
        {
            // Check user preferences
            var isEmailEnabled = await _preferenceRepo.IsEnabledAsync(evt.UserId, NotificationChannel.Email, ct);
            var isPushEnabled = await _preferenceRepo.IsEnabledAsync(evt.UserId, NotificationChannel.Push, ct);

            if (!isEmailEnabled && !isPushEnabled) return;

            if (isEmailEnabled)
            {
                var notification = Notification.Create(
                    evt.UserId,
                    NotificationChannel.Email,
                    "Wallet Recharged",
                    $"Your wallet has been successfully recharged with {evt.Amount:C} at {evt.RechargedAt:yyyy-MM-dd HH:mm}.",
                                evt.phone,
            evt.Email
                );
                await _notificationRepo.AddAsync(notification, ct);
            }

            if (isPushEnabled)
            {
                var notification = Notification.Create(
                    evt.UserId,
                    NotificationChannel.Push,
                    "Wallet Recharged",
                    $"Your wallet has been recharged with {evt.Amount:C}.",
                                evt.phone,
            evt.Email
                );
                await _notificationRepo.AddAsync(notification, ct);
            }
        }
    }

}
