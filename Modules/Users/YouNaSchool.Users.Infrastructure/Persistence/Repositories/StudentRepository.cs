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
    public class StudentRepository : Repository<Student>, IStudentRepository
    {
        private readonly UserDbContext _context;
        public StudentRepository(UserDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<Student>> GetByAssistantIdAsync(Guid assistantId, CancellationToken cancellationToken = default)
        {
            var students = await _context.Students
                .Where(s => s.AssistantId == assistantId)
                .ToListAsync(cancellationToken);
            return students;
        }

        public async Task<Student?> GetByUserIdAsync(Guid userId, bool asTracking = true, CancellationToken cancellationToken = default)
        {
            var query = _context.Students.AsQueryable();
            if (!asTracking)
            {
                query = query.AsNoTracking();
            }
            var student = await query.FirstOrDefaultAsync(s => s.UserId == userId, cancellationToken);
            return student;
        }
    }
}
