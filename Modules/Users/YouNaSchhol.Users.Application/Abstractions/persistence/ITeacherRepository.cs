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
    /// Repository abstraction for managing Teacher aggregates.
    /// </summary>
    public interface ITeacherRepository : IRepository<Teacher>
    {
        Task<Teacher?> GetByUserIdAsync(
                   Guid userId,
                   bool asTracking = true,
                   CancellationToken cancellationToken = default);
        Task<Teacher?> GetTeacherOfAssistantAsync(Guid assistantId);
    }
}
