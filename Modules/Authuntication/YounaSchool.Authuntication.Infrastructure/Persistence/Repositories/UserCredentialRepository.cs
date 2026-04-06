using Microsoft.EntityFrameworkCore;
using SharedKernel.Infrastructure.Persistence;
using YounaSchool.Authentication.Domain.Aggregates;
using YounaSchool.Authuntication.Domain.Interfaces.Repositories;
using YounaSchool.Authuntication.Infrastructure.Persistence;

namespace YounaSchool.Authuntication.Infrastructure.Persistence.Repositories;

internal sealed class UserCredentialRepository : Repository<UserCredential>, IUserCredentialRepository
{
    private readonly AuthDbContext _context;

    public UserCredentialRepository(AuthDbContext context)
        : base(context)
    {
        _context = context;
    }

    public async Task<UserCredential?> GetByUserIdAsync(Guid userId, bool asTracked = true, CancellationToken cancellationToken = default)
    {
        var query = _context.UserCredentials.AsQueryable();

        if (!asTracked)
            query = query.AsNoTracking();

        return await query
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);
    }
}
