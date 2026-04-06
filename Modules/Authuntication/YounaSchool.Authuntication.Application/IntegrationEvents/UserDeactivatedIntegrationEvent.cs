using Shared.Application.IntegrationEvents;

namespace YounaSchool.Authuntication.Application.IntegrationEvents;

/// <summary>
/// Published when a user's account is locked/deactivated
/// </summary>
public sealed record UserAccountSuspendIntegrationEvent(
    Guid UserId,
    string Email,
    DateTime LockedAt) : IIntegrationEvent
{
    public DateTime OccurredOnUtc => DateTime.UtcNow;

    public Guid EventId => Guid.NewGuid();
}