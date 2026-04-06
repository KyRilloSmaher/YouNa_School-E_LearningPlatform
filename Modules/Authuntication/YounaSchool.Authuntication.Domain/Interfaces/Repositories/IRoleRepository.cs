using SharedKernel.Domain.CoreAbstractions;
using YounaSchool.Authentication.Domain.Aggregates;

namespace YounaSchool.Authuntication.Domain.Interfaces.Repositories;

public interface IRoleRepository : IRepository<Role>
{
    Task<Role?> GetByNameAsync(string name, bool asTracked = true, CancellationToken cancellationToken = default);
}

