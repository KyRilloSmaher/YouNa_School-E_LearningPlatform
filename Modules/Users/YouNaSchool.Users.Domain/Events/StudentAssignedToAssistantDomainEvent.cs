using SharedKernel.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YounaSchool.Users.Domain.Events
{
    public class StudentAssignedToAssistantDomainEvent : DomainEvent
    {
        public Guid StudentId { get; }
        public Guid AssistantId { get; }
        public StudentAssignedToAssistantDomainEvent(Guid studentId, Guid assistantId)
        {
            StudentId = studentId;
            AssistantId = assistantId;
        }
    
    }
}
