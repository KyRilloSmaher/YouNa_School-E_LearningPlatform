using SharedKernel.Domain.Events;

namespace YounaSchool.Authuntication.Domain.Events;

/// <summary>
/// Raised when a new role is created in the Auth module.
/// </summary>
public sealed class RoleCreatedEvent : DomainEvent
{
    public Guid RoleId { get; }
    public string Name { get; }

    public RoleCreatedEvent(Guid roleId, string name)
    {
        RoleId = roleId;
        Name = name;
    }
}

