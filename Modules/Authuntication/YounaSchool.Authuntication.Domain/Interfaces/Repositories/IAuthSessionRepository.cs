using SharedKernel.Domain.CoreAbstractions;
using YounaSchool.Authuntication.Domain.Aggregates;

namespace YounaSchool.Authuntication.Domain.Interfaces.Repositories;

public interface IAuthSessionRepository : IRepository<AuthSession>
{
    Task<IReadOnlyCollection<AuthSession>> GetActiveSessionsForUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task RevokeAllSessionsForUserAsync(Guid userId, CancellationToken cancellationToken = default);
}

