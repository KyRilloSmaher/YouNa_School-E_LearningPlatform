using SharedKernel.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YounaSchool.Users.Domain.Events
{
    public class TeacherVerifiedDomainEvent  : DomainEvent
    {
        public Guid TeacherId { get; }
        public TeacherVerifiedDomainEvent(Guid teacherId)
        {
            TeacherId = teacherId;
        }
    }
}
