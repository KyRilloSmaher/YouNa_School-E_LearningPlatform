using SharedKernel.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YounaSchool.Users.Domain.Events
{
    public sealed class StudentSuspendedEvent : DomainEvent
    {
        public Guid StudentId { get; }
        public string Reason { get; }

        public StudentSuspendedEvent(Guid studentId, string reason)
        {
            StudentId = studentId;
            Reason = reason;
        }
    }
}
