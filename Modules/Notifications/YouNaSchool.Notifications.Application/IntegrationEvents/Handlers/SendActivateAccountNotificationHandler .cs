using MediatR;
using YouNaSchool.Notifications.Domain.Enums;
using YouNaSchool.Notifications.Domain.Interfaces.Repositories;
using YouNaSchool.Notifications.Domain.Models;

namespace YouNaSchool.Notifications.Application.IntegrationEvents.Handlers
{


    public class SendActivateAccountNotificationHandler : IIntegrationEventHandler<UserActivatedIntegrationEvent>
    {
        private readonly INotificationRepository _notificationRepo;
        private readonly INotificationPreferenceRepository _preferenceRepo;

        public SendActivateAccountNotificationHandler(
            INotificationRepository notificationRepo,
            INotificationPreferenceRepository preferenceRepo)
        {
            _notificationRepo = notificationRepo;
            _preferenceRepo = preferenceRepo;
        }

        public async Task HandleAsync(UserActivatedIntegrationEvent evt, CancellationToken ct)
        {
            // Check user preference
            var isEmailEnabled = await _preferenceRepo.IsEnabledAsync(evt.UserId, NotificationChannel.Email, ct);
            if (!isEmailEnabled) return;

            var notification = Notification.Create(
                evt.UserId,
                NotificationChannel.Email,
                "Account Activated",
                $"Your account has been successfully activated at {evt.ActivatedAt:yyyy-MM-dd HH:mm}.",
                evt.phone,evt.Email
            );

            await _notificationRepo.AddAsync(notification, ct);
        }
    }
}
