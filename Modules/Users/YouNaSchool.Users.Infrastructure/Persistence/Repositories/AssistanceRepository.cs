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
    public class AssistantRepository : Repository<Assistant>, IAssistantRepository
    {
        private readonly UserDbContext _context;
        public AssistantRepository(UserDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<IQueryable<Assistant>> GetAvailableAssistanceForTeacherAsync(Guid TeacherId,bool asTracked = true,CancellationToken cancellationToken = default)
        {
            // First, get the maximum student count from the table
            var maxStudentCount = await _context.Students
                .GroupBy(s => s.AssistantId)
                .Select(g => g.Count())
                .MaxAsync(cancellationToken);

            // Then find assistants with student count below that maximum
            var assistantIds = await _context.Students
                .GroupBy(s => s.AssistantId)
                .Select(g => new
                {
                    AssistantId = g.Key,
                    StudentCount = g.Count()
                })
                .Where(x => x.StudentCount < maxStudentCount)
                .Select(x => x.AssistantId)
                .ToListAsync(cancellationToken);

            // Build the query for assistants
            var assistantsQuery = _context.Assistants
                .Where(s => s.TeacherId == TeacherId)
                .Where(a => assistantIds.Contains(a.Id));


            return assistantsQuery;
        }

        public async Task<Assistant?> GetAvailableAssistantAsync(Guid teacherId, bool asTracked = true, CancellationToken cancellationToken = default)
        {
            var assistances = await GetAvailableAssistanceForTeacherAsync(teacherId, asTracked, cancellationToken);

            return await assistances.FirstOrDefaultAsync(a=> a.TeacherId == teacherId, cancellationToken);                
        }

        public async Task<IQueryable<Assistant>> GetByTeacherIdAsync(Guid teacherId, CancellationToken cancellationToken = default)
        {
            var assistants = _context.Assistants.Where(a => a.TeacherId == teacherId);
            return assistants;
        }

        public async Task<Assistant?> GetByUserIdAsync(Guid userId, bool asTracked = true, CancellationToken cancellationToken = default)
        {
            var query = _context.Assistants.AsQueryable();
            if (!asTracked)
            {
                query = query.AsNoTracking();
            }
             return await query.FirstOrDefaultAsync(a => a.UserId == userId);

        }
    }
}
