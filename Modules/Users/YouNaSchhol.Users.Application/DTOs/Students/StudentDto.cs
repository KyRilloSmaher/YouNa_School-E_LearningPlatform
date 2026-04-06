using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YounaSchool.Users.Domain.Enums;

namespace YouNaSchhol.Users.Application.DTOs.Students
{
    public class StudentDto 
    {
        public Guid Id { get; init; }
        public Guid UserId { get; init; }
        public Level Level { get; init; }
        public int SuspensionCount { get; init; }
        public bool IsSuspended { get; init; }


    }
}
