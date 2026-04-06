using Microsoft.EntityFrameworkCore;
using SharedKernel.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouNaSchool.Users.Application.Abstractions.Persistence;
using YouNaSchool.Users.Domain.Entities;

namespace YouNaSchool.Users.Infrastructure.Persistence.Repositories
{
    public class TeacherRepository : Repository <Teacher> ,ITeacherRepository 
    {
        private readonly UserDbContext _context;
        public TeacherRepository(UserDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Teacher?> GetByUserIdAsync(Guid userId, bool asTracking, CancellationToken cancellationToken)
        {
            var query = _context.Teachers.Where(t=>t.UserId == userId);
            if (!asTracking)
            {
                query = query.AsNoTracking();
            }
            return await query.FirstOrDefaultAsync(cancellationToken);

        }

        public async  Task<Teacher?> GetTeacherOfAssistantAsync(Guid assistantId)
        {
            var assistant = await _context.Assistants.FirstOrDefaultAsync(a => a.Id == assistantId);
            if (assistant == null)
                throw new InvalidOperationException($"Assistant with ID {assistantId} not found.");
            var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.Id == assistant.TeacherId);
            return teacher;
        }
    }
}
