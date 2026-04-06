using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YounaSchool.Authentication.Domain.Aggregates;
using YounaSchool.Authuntication.Domain.Aggregates;

namespace YounaSchool.Authuntication.Infrastructure.Persistence.Configurations;

internal sealed class UserCredentialConfiguration : IEntityTypeConfiguration<UserCredential>
{
    public void Configure(EntityTypeBuilder<UserCredential> builder)
    {
        builder.ToTable("UserCredentials");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId)
            .IsRequired();

        // Owned type for HashedPassword value object
        builder.OwnsOne(
            x => x.Password,
            p =>
            {
                p.Property(h => h.Value)
                    .HasColumnName("PasswordHash")
                    .HasMaxLength(500)
                    .IsRequired();
            });

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        // Password history - use backing field (persisted as semicolon-separated)
        builder.Property<List<string>>("_passwordHistory")
            .HasConversion(
                v => string.Join(";", v),
                v => v.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList())
            .HasColumnName("PasswordHistory")
            .HasMaxLength(2000);

        builder.HasIndex(x => x.UserId)
            .IsUnique();
    }
}
