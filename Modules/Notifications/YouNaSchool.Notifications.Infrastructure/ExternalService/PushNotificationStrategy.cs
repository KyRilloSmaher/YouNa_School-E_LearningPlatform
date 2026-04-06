using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouNaSchool.Notifications.Domain.Domain_Events.YouNaSchool.Notifications.Domain.Events;
using YouNaSchool.Notifications.Domain.Interfaces.Services;

namespace YouNaSchool.Notifications.Infrastructure.ExternalService
{
    // Push notification strategy implementation
    public class PushNotificationStrategy : INotificationChannelStrategy
    {
        private readonly INotificationSender _notificationSender;

        public PushNotificationStrategy(INotificationSender notificationSender)
        {
            _notificationSender = notificationSender;
        }

        public async Task SendAsync(NotificationCreatedEvent notificationEvent, CancellationToken ct)
        {
            await _notificationSender.SendAsync(
                notificationEvent.NotificationId,
                notificationEvent.UserId,
                notificationEvent.Title ?? "New Notification",
                notificationEvent.Message,
                ct);
        }
    }
}
