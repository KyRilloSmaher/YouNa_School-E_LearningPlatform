using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YounaSchool.Authentication.Domain.Aggregates;
using YounaSchool.Authentication.Domain.ValueObjects;

namespace YounaSchool.Authuntication.Infrastructure.Persistence.Configurations;

internal sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");

        builder.HasKey(x => x.Id);
        builder.Ignore(x => x.Permissions);

        builder.Property(x => x.Name)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        // Permissions stored as JSON (Resource/Action pairs)
        builder.Property<List<Permission>>("_permissions")
            .HasConversion(
                v => JsonSerializer.Serialize(v.Select(p => new PermissionDto(p.Resource, p.Action)).ToList(), JsonOptions),
                v => DeserializePermissions(v))
            .HasColumnName("Permissions")
            .HasColumnType("nvarchar(max)");

        builder.HasIndex(x => x.Name)
            .IsUnique();
    }

    private static List<Permission> DeserializePermissions(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return new List<Permission>();

        var dtos = JsonSerializer.Deserialize<List<PermissionDto>>(json, JsonOptions);
        if (dtos is null)
            return new List<Permission>();

        var result = new List<Permission>();
        foreach (var dto in dtos)
        {
            var permResult = Permission.Create(dto.Resource ?? "", dto.Action ?? "");
            if (permResult.IsSuccess && permResult.Value is not null)
                result.Add(permResult.Value);
        }
        return result;
    }

    private record PermissionDto(string Resource, string Action);
}
