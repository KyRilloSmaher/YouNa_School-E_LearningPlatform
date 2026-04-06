using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using YounaSchool.Authuntication.Domain.Aggregates;
using YounaSchool.Authentication.Domain.ValueObjects;

namespace YounaSchool.Authuntication.Infrastructure.Persistence.Configurations;

internal sealed class AuthSessionConfiguration : IEntityTypeConfiguration<AuthSession>
{
    public void Configure(EntityTypeBuilder<AuthSession> builder)
    {
        builder.ToTable("AuthSessions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId)
            .IsRequired();

        // Owned type for RefreshToken value object
        builder.OwnsOne(
            x => x.RefreshToken,
            r =>
            {
                r.Property(rt => rt.TokenHash)
                    .HasColumnName("RefreshTokenHash")
                    .HasMaxLength(500)
                    .IsRequired();
                r.Property(rt => rt.ExpiresAtUtc)
                    .HasColumnName("RefreshTokenExpiresAtUtc")
                    .IsRequired();
            });

        builder.Property(x => x.Status)
            .HasConversion(new EnumToStringConverter<YounaSchool.Authentication.Domain.Enums.SessionStatus>())
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.DeviceInfo)
            .HasMaxLength(500);

        builder.Property(x => x.IpAddress)
            .HasMaxLength(100);

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => new { x.UserId, x.Status });
    }
}
