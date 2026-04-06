using SharedKernel.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouNaSchool.Notifications.Domain.Enums;

namespace YouNaSchool.Notifications.Domain.Domain_Events
{

    namespace YouNaSchool.Notifications.Domain.Events
    {
        public sealed record NotificationCreatedEvent(
            Guid NotificationId,
            Guid UserId,
            string Title,
            string Message,
            NotificationChannel Channel, string? phone, string? Email
        ) : IDomainEvent
        {
            public DateTime OccurredOn => DateTime.UtcNow;
        }
    }
}
