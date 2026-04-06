using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouNaSchool.Notifications.Domain.Domain_Events.YouNaSchool.Notifications.Domain.Events;

namespace YouNaSchool.Notifications.Infrastructure.ExternalService
{
    // Strategy interface
    public interface INotificationChannelStrategy
    {
        Task SendAsync(NotificationCreatedEvent notificationEvent, CancellationToken ct);
    }
}
