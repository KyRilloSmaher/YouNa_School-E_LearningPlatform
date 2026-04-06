using SharedKernel.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YounaSchool.Authuntication.Domain.Events
{
    public class UserLoggedInDomainEvent : DomainEvent
    {
        public Guid UserId { get; }
        public UserLoggedInDomainEvent(Guid userId)
        {
            UserId = userId;
        }
    
    }
}
