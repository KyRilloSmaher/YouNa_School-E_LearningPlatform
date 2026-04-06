using SharedKernel.Domain.CoreAbstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouNaSchool.Users.Domain.Entities;

namespace YouNaSchool.Users.Application.Abstractions.Persistence
{
    /// <summary>
    /// Repository abstraction for managing Assistant aggregates.
    /// </summary>
    public interface IAssistantRepository : IRepository<Assistant>
    {
        Task<Assistant?> GetByUserIdAsync(Guid userId,bool asTracked = true,CancellationToken cancellationToken = default);
        Task<IQueryable<Assistant>> GetByTeacherIdAsync(Guid teacherId,CancellationToken cancellationToken = default);
        Task<IQueryable<Assistant>> GetAvailableAssistanceForTeacherAsync(Guid TeacherId, bool asTracked = true, CancellationToken cancellationToken = default);
        Task<Assistant?>  GetAvailableAssistantAsync(Guid teacherId, bool asTracked = true, CancellationToken cancellationToken = default);
    }
}
