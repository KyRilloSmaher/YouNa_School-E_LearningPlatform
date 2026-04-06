using SharedKernel.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YounaSchool.Users.Domain.Events
{
    public sealed class UserActivatedEvent : DomainEvent
    {
        public Guid UserId { get; }
        public DateTime OccurredOn { get; } = DateTime.UtcNow;

        public UserActivatedEvent(Guid userId)
        {
            UserId = userId;
        }
    }
}
