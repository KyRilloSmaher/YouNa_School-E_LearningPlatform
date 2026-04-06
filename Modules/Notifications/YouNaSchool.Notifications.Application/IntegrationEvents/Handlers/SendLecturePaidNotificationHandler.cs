using MediatR;
using YouNaSchool.Notifications.Domain.Enums;
using YouNaSchool.Notifications.Domain.Interfaces.Repositories;
using YouNaSchool.Notifications.Domain.Models;

namespace YouNaSchool.Notifications.Application.IntegrationEvents.Handlers
{


    public class SendLecturePaidNotificationHandler : IIntegrationEventHandler<LecturePaidIntegrationEvent>
    {
        private readonly INotificationRepository _notificationRepo;
        private readonly INotificationPreferenceRepository _preferenceRepo;

        public SendLecturePaidNotificationHandler(
            INotificationRepository notificationRepo,
            INotificationPreferenceRepository preferenceRepo)
        {
            _notificationRepo = notificationRepo;
            _preferenceRepo = preferenceRepo;
        }

        public async Task HandleAsync(LecturePaidIntegrationEvent evt, CancellationToken ct)
        {
            // Check user preferences
            var isPushEnabled = await _preferenceRepo.IsEnabledAsync(evt.UserId, NotificationChannel.Push, ct);
            var isEmailEnabled = await _preferenceRepo.IsEnabledAsync(evt.UserId, NotificationChannel.Email, ct);

            if (!isPushEnabled && !isEmailEnabled) return;

            if (isPushEnabled)
            {
                var notification = Notification.Create(
                    evt.UserId,
                    NotificationChannel.Push,
                    "Lecture Paid",
                    $"You have successfully paid for lecture {evt.LectureId} at {evt.PaidAt:yyyy-MM-dd HH:mm}.",
                         evt.phone,
                        evt.Email
                );
                await _notificationRepo.AddAsync(notification, ct);
            }

            if (isEmailEnabled)
            {
                var notification = Notification.Create(
                    evt.UserId,
                    NotificationChannel.Email,
                    "Lecture Payment Success",
                    $"Your payment for lecture {evt.LectureId} of {evt.Amount:C} has been processed successfully.",
                    evt.phone,
                    evt.Email
                );
                await _notificationRepo.AddAsync(notification, ct);
            }
        }
    }
}
