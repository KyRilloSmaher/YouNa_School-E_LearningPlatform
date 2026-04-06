using SharedKernel.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouNaSchool.Notifications.Domain.Domain_Events
{
    public sealed record NotificationReadEvent(
     Guid NotificationId,
     Guid UserId
 ) : IDomainEvent
    {
        public DateTime OccurredOn => DateTime.UtcNow;
    }
}
