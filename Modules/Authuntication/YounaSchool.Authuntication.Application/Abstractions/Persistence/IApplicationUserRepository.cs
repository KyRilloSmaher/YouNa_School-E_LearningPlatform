namespace YounaSchool.Authuntication.Application.Abstractions.Persistence;

/// <summary>
/// Repository for Identity User (ApplicationUser) operations.
/// </summary>
public interface IApplicationUserRepository
{
    Task<AuthUserInfo?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<AuthUserInfo?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Guid> AddAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task MarkEmailConfirmedAsync(Guid userId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Read model for authenticated user info.
/// </summary>
public record AuthUserInfo(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string Role,
    bool IsActive,
    bool EmailConfirmed,
    string? ConfirmationToken,
    DateTime? ConfirmationTokenExpiresAtUtc);

/// <summary>
/// Request to create a new user.
/// </summary>
public record CreateUserRequest(
    string Email,
    string FirstName,
    string LastName,
    string RoleName, // e.g. "Student", "Teacher"
    string PasswordHash,
    string ConfirmationToken,
    DateTime ConfirmationTokenExpiresAtUtc);
