using Shared.Application.IntegrationEvents;

namespace Auth.Application.IntegrationEvents;

/// <summary>
/// Published when a user Account successfully Verfied
/// </summary>
public sealed record UserActivatedInIntegrationEvent(Guid UserId,string Email) : IIntegrationEvent
{
    public DateTime OccurredOnUtc => DateTime.UtcNow;

    public Guid EventId => Guid.NewGuid();
}


