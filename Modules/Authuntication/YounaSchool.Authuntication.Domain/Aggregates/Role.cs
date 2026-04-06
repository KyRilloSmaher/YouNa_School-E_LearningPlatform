using Shared.Application.RESULT_PATTERN;
using SharedKernel.Domain.CoreAbstractions;
using System.Net;
using YounaSchool.Authentication.Domain.ValueObjects;
using YounaSchool.Authuntication.Domain.Events;

namespace YounaSchool.Authentication.Domain.Aggregates;

/// <summary>
/// Represents a role that groups a set of permissions.
/// </summary>
public sealed class Role : AggregateRoot
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }

    private List<Permission> _permissions = new();
    public IReadOnlyCollection<Permission> Permissions => _permissions.AsReadOnly();

    private Role() { }

    private Role(Guid id, string name, string? description, IEnumerable<Permission> permissions)
    {
        Id = id;
        Name = name;
        Description = description;
        _permissions.AddRange(permissions);

        RaiseDomainEvent(new RoleCreatedEvent(Id, Name));
    }

    public static Result<Role> Create(string name, string? description, IEnumerable<Permission> permissions)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result<Role>.Failure("Role name is required.", HttpStatusCode.BadRequest);
        }

        var perms = permissions?.ToList() ?? new List<Permission>();

        var role = new Role(
            Guid.NewGuid(),
            name.Trim(),
            description?.Trim(),
            perms);

        return Result<Role>.Success(role);
    }

    public Result<bool> UpdatePermissions(IEnumerable<Permission> permissions)
    {
        var newPermissions = permissions?.ToList() ?? new List<Permission>();

        _permissions.Clear();
        _permissions.AddRange(newPermissions);

        RaiseDomainEvent(new PermissionsUpdatedEvent(Id));

        return Result<bool>.Success(true);
    }
}

