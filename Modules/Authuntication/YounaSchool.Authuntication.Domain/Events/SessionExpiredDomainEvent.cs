using SharedKernel.Domain.Events;

namespace YounaSchool.Authuntication.Domain.Events;

/// <summary>
/// Raised when an authentication session has expired (typically when the refresh token expires).
/// </summary>
public sealed class SessionExpiredDomainEvent : DomainEvent
{
    public Guid SessionId { get; }

    public SessionExpiredDomainEvent(Guid sessionId)
    {
        SessionId = sessionId;
    }
}

