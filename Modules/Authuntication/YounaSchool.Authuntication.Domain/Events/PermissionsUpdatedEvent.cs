using SharedKernel.Domain.Events;

namespace YounaSchool.Authuntication.Domain.Events;

/// <summary>
/// Raised when the permissions assigned to a role are changed.
/// </summary>
public sealed class PermissionsUpdatedEvent : DomainEvent
{
    public Guid RoleId { get; }

    public PermissionsUpdatedEvent(Guid roleId)
    {
        RoleId = roleId;
    }
}

