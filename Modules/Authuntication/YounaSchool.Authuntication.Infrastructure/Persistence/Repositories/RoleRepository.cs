using Microsoft.EntityFrameworkCore;
using SharedKernel.Infrastructure.Persistence;
using YounaSchool.Authentication.Domain.Aggregates;
using YounaSchool.Authuntication.Domain.Interfaces.Repositories;
using YounaSchool.Authuntication.Infrastructure.Persistence;

namespace YounaSchool.Authuntication.Infrastructure.Persistence.Repositories;

internal sealed class RoleRepository : Repository<Role>, IRoleRepository
{
    private readonly AuthDbContext _context;

    public RoleRepository(AuthDbContext context)
        : base(context)
    {
        _context = context;
    }

    public async Task<Role?> GetByNameAsync(string name, bool asTracked = true, CancellationToken cancellationToken = default)
    {
        var normalized = name.Trim();
        var query = _context.Roles.AsQueryable();

        if (!asTracked)
            query = query.AsNoTracking();

        return await query
            .FirstOrDefaultAsync(r => r.Name == normalized, cancellationToken);
    }
}
