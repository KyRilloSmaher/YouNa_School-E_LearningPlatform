using SharedKernel.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YounaSchool.Authuntication.Domain.Events
{
    public class PasswordChangedDomainEvent : DomainEvent
    {
        public Guid UserId { get; }
        public PasswordChangedDomainEvent(Guid userId)
        {
            UserId = userId;
        }
    
    }
}
