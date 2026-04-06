using SharedKernel.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YounaSchool.Authuntication.Domain.Events
{
    public class SessionRevokedDomainEvent : DomainEvent
    {
        public Guid SessionId { get; }
        public string Reason { get; }
        public SessionRevokedDomainEvent(Guid sessionId, string reason)
        {
            SessionId = sessionId;
            Reason = reason;
        }
    
    }
}
