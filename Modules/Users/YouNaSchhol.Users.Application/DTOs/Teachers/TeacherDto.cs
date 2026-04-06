using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouNaSchhol.Users.Application.DTOs.Teachers
{
    public class TeacherDto
    {
        public Guid Id { get; private set; }

        /// <summary>
        /// Reference to the Identity user (Auth module).
        /// </summary>
        public Guid UserId { get; private set; }

        /// <summary>
        /// Indicates whether the teacher account has been verified.
        /// </summary>
        public bool IsVerified { get; private set; }
    }
}
