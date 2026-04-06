using SharedKernel.Domain.Events;
using SharedKernel.Domain.VALUE_OBJECTS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YounaSchool.Authuntication.Domain.Enums;

namespace YounaSchool.Authuntication.Domain.Events
{
    public class UserRegisteredEvent : DomainEvent
    {
        public Guid UserId { get; }
        public Email Email { get; }
        public UserRole Role { get; }
        public UserRegisteredEvent(Guid userId, Email email, UserRole role)
        {
            UserId = userId;
            Email = email;
            Role = role;
        }
    
    }
}
