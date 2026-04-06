using Microsoft.AspNetCore.Identity;

namespace YounaSchool.Authuntication.Infrastructure.Identity;

/// <summary>
/// Application role entity that extends ASP.NET Core Identity's IdentityRole.
/// Integrates domain role concept with Identity for authorization.
/// </summary>
public sealed class ApplicationRole : IdentityRole<Guid>
{
    public string? Description { get; private set; }

    private ApplicationRole() { }

    public ApplicationRole(string name, string? description = null)
    {
        Id = Guid.NewGuid();
        Name = name;
        NormalizedName = name.ToUpperInvariant();
        Description = description;
    }

    public void SetDescription(string? description)
    {
        Description = description;
    }
}
