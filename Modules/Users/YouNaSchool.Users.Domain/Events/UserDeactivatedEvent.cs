using SharedKernel.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YounaSchool.Users.Domain.Events
{
    public sealed class UserDeactivatedEvent : DomainEvent
    {
        public Guid UserId { get; }
        public string Reason { get; }
        public DateTime OccurredOn { get; } = DateTime.UtcNow;

        public UserDeactivatedEvent(Guid userId, string reason)
        {
            UserId = userId;
            Reason = reason;
        }
    }
}
