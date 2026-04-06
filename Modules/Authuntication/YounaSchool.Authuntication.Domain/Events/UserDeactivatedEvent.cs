using SharedKernel.Domain.Events;
using SharedKernel.Domain.VALUE_OBJECTS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YounaSchool.Authuntication.Domain.Events
{
    public class UserDeactivatedEvent : DomainEvent
    {
        public Guid UserId { get; }
        public Email Email { get; }

        public UserDeactivatedEvent(Guid userId, Email email)
        {
            UserId = userId;
            Email = email;
        }
    }
}
