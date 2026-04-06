using Microsoft.EntityFrameworkCore;
using SharedKernel.Infrastructure.Persistence;
using YounaSchool.Authentication.Domain.Enums;
using YounaSchool.Authuntication.Domain.Aggregates;
using YounaSchool.Authuntication.Domain.Interfaces.Repositories;
using YounaSchool.Authuntication.Infrastructure.Persistence;

namespace YounaSchool.Authuntication.Infrastructure.Persistence.Repositories;

internal sealed class AuthSessionRepository : Repository<AuthSession>, IAuthSessionRepository
{
    private readonly AuthDbContext _context;

    public AuthSessionRepository(AuthDbContext context)
        : base(context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<AuthSession>> GetActiveSessionsForUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.AuthSessions
            .AsNoTracking()
            .Where(s => s.UserId == userId && s.Status == SessionStatus.Active)
            .ToListAsync(cancellationToken);
    }

    public async Task RevokeAllSessionsForUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var sessions = await _context.AuthSessions
            .Where(s => s.UserId == userId && s.Status == SessionStatus.Active)
            .ToListAsync(cancellationToken);

        foreach (var session in sessions)
        {
            session.Revoke("Revoked by system");
        }
    }
}
