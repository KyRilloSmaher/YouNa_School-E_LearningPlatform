using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouNaSchool.Notifications.Domain.Domain_Events.YouNaSchool.Notifications.Domain.Events;
using YouNaSchool.Notifications.Domain.Interfaces.Services;

namespace YouNaSchool.Notifications.Infrastructure.ExternalService
{

    // SMS strategy implementation
    public class SmsNotificationStrategy : INotificationChannelStrategy
    {
        private readonly ISmsService _smsService;

        public SmsNotificationStrategy(ISmsService smsService)
        {
            _smsService = smsService;
        }

        public async Task SendAsync(NotificationCreatedEvent notificationEvent, CancellationToken ct)
        {
            if (string.IsNullOrEmpty(notificationEvent.phone))
                throw new InvalidOperationException("Phone number is required for SMS notifications");

            await _smsService.SendSmsAsync(
                notificationEvent.phone,
                notificationEvent.Message);
        }
    }
}
