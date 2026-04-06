using SharedKernel.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YounaSchool.Authuntication.Domain.Events
{
    public class UserActivatedEvent : DomainEvent
    {
        public Guid UserId { get; }
        public string? Email { get; }
        public UserActivatedEvent(Guid userId, string? email)
        {
            UserId = userId;
            Email = email;
        }
    }
}
