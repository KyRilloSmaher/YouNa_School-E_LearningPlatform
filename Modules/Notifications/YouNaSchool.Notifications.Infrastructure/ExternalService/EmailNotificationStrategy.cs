using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouNaSchool.Notifications.Domain.Domain_Events.YouNaSchool.Notifications.Domain.Events;
using YouNaSchool.Notifications.Domain.Interfaces.Services;

namespace YouNaSchool.Notifications.Infrastructure.ExternalService
{
    // Email strategy implementation
    public class EmailNotificationStrategy : INotificationChannelStrategy
    {
        private readonly IEmailService _emailService;

        public EmailNotificationStrategy(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task SendAsync(NotificationCreatedEvent notificationEvent, CancellationToken ct)
        {
            if (string.IsNullOrEmpty(notificationEvent.Email))
                throw new InvalidOperationException("Recipient email is required for email notifications");

            await _emailService.SendEmailAsync(
                notificationEvent.Email,
                notificationEvent.Title ?? "Notification",
                notificationEvent.Message
                );
        }
    }
}
