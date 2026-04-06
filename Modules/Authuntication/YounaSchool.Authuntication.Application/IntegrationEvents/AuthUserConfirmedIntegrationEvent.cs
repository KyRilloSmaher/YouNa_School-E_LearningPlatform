using Shared.Application.IntegrationEvents;

namespace YounaSchool.Authuntication.Application.IntegrationEvents;

public sealed record AuthUserConfirmedIntegrationEvent(
    Guid UserId,
    string Email,
    string FirstName,
    string LastName,
    string Role
) : IntegrationEvent;
