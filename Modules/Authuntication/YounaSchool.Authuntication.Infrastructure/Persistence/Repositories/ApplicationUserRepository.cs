using Microsoft.EntityFrameworkCore;
using YounaSchool.Authuntication.Application.Abstractions.Persistence;
using YounaSchool.Authuntication.Domain.Enums;
using YounaSchool.Authuntication.Infrastructure.Identity;
using YounaSchool.Authuntication.Infrastructure.Persistence;

namespace YounaSchool.Authuntication.Infrastructure.Persistence.Repositories;

internal sealed class ApplicationUserRepository : IApplicationUserRepository
{
    private readonly AuthDbContext _context;

    public ApplicationUserRepository(AuthDbContext context)
    {
        _context = context;
    }

    public async Task<AuthUserInfo?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        return user is null ? null : MapToInfo(user);
    }

    public async Task<AuthUserInfo?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalized = email.Trim().ToLowerInvariant();
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.NormalizedEmail == normalized.ToUpperInvariant(), cancellationToken);

        return user is null ? null : MapToInfo(user);
    }

    public async Task<Guid> AddAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        if (!Enum.TryParse<UserRole>(request.RoleName, ignoreCase: true, out var role))
            role = UserRole.Student;

        var user = new ApplicationUser(
            request.Email.Trim().ToLowerInvariant(),
            request.FirstName.Trim(),
            request.LastName.Trim(),
            role)
        {
            PasswordHash = request.PasswordHash
        };
        user.SetEmailConfirmation(request.ConfirmationToken, request.ConfirmationTokenExpiresAtUtc);

        _context.Users.Add(user);
        return user.Id;
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalized = email.Trim().ToLowerInvariant();
        return await _context.Users
            .AsNoTracking()
            .AnyAsync(u => u.NormalizedEmail == normalized.ToUpperInvariant(), cancellationToken);
    }

    public async Task MarkEmailConfirmedAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user is null)
        {
            return;
        }

        user.ConfirmEmail();
    }

    private static AuthUserInfo MapToInfo(ApplicationUser user) =>
        new(
            user.Id,
            user.Email ?? "",
            user.FirstName,
            user.LastName,
            user.Role.ToString(),
            user.IsActive,
            user.EmailConfirmed,
            user.ConfirmationToken,
            user.ConfirmationTokenExpiresAtUtc);
}
