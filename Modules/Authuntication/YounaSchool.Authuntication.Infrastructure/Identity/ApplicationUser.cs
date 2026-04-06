using Microsoft.AspNetCore.Identity;
using YounaSchool.Authuntication.Domain.Enums;

namespace YounaSchool.Authuntication.Infrastructure.Identity;

/// <summary>
/// Application user entity that extends ASP.NET Core Identity's IdentityUser.
/// Integrates domain user concept with Identity for authentication and authorization.
/// </summary>
public sealed class ApplicationUser : IdentityUser<Guid>
{
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public UserRole Role { get; private set; }

    public bool IsActive { get; private set; }
    public bool IsDeleted { get; private set; }
    public string? ConfirmationToken { get; private set; }
    public DateTime? ConfirmationTokenExpiresAtUtc { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public DateTime? LastLoginAt { get; private set; }

    private ApplicationUser() { }

    public ApplicationUser(
        string email,
        string firstName,
        string lastName,
        UserRole role)
    {
        Id = Guid.NewGuid();
        UserName = email;
        NormalizedUserName = email.ToUpperInvariant();
        Email = email;
        NormalizedEmail = email.ToUpperInvariant();
        FirstName = firstName;
        LastName = lastName;
        Role = role;
        IsActive = false;
        IsDeleted = false;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        EmailConfirmed = false;
    }

    public void SetEmailConfirmation(string token, DateTime expiresAtUtc)
    {
        ConfirmationToken = token;
        ConfirmationTokenExpiresAtUtc = expiresAtUtc;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ConfirmEmail()
    {
        EmailConfirmed = true;
        IsActive = true;
        ConfirmationToken = null;
        ConfirmationTokenExpiresAtUtc = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        IsDeleted = true;
        UpdatedAt = DateTime.UtcNow;
    }


    public void RecordLogin()
    {
        LastLoginAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
