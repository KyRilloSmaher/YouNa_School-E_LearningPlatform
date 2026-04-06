using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Application.OUTBOX_PATTERN;
using YounaSchool.Authentication.Domain.Aggregates;
using YounaSchool.Authuntication.Domain.Aggregates;
using YounaSchool.Authuntication.Infrastructure.Identity;

namespace YounaSchool.Authuntication.Infrastructure.Persistence;

/// <summary>
/// Database context for the Authentication module.
/// Integrates ASP.NET Core Identity with domain aggregates (AuthSession, UserCredential, Role).
/// </summary>
public sealed class AuthDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options)
        : base(options)
    {
    }

    public DbSet<AuthSession> AuthSessions => Set<AuthSession>();
    public DbSet<UserCredential> UserCredentials => Set<UserCredential>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Auth");
        base.OnModelCreating(modelBuilder);


        // Apply all configurations from this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AuthDbContext).Assembly);

    }
}
