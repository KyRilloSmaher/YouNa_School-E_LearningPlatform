using SharedKernel.Domain.CoreAbstractions;
using YouNaSchool.Users.Domain.Entities;

namespace YouNaSchool.Users.Application.Abstractions.Persistence;

/// <summary>
/// Repository abstraction for managing Student aggregates.
/// </summary>
public interface IStudentRepository : IRepository<Student>
{
    Task<Student?> GetByUserIdAsync( Guid userId,bool asTracking = true,CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Student>> GetByAssistantIdAsync(Guid assistantId,CancellationToken cancellationToken = default);

}