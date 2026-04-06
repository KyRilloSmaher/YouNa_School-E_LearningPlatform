using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouNaSchhol.Users.Application.DTOs.Assistants
{
    public class AssistantDto
    {
        public Guid Id { get; private set; }

        /// <summary>
        /// Reference to the Identity user (Auth module).
        /// </summary>
        public Guid UserId { get; private set; }

        /// <summary>
        /// The teacher supervising this assistant.
        /// </summary>
        public Guid TeacherId { get; private set; }
    }
}
