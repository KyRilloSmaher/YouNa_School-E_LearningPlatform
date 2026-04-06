using SharedKernel.Domain.CoreAbstractions;
using YounaSchool.Authentication.Domain.Aggregates;

namespace YounaSchool.Authuntication.Domain.Interfaces.Repositories;

public interface IUserCredentialRepository : IRepository<UserCredential>
{
    Task<UserCredential?> GetByUserIdAsync(Guid userId, bool asTracked = true, CancellationToken cancellationToken = default);
}

