using Shared.Application.IntegrationEvents;

namespace YouNaSchhol.Users.Application.IntegrationEvents;

public sealed record StudentCreatedIntegrationEvent(
    Guid UserId,
    string Email,
    string FullName,
    DateTime CreatedAt
) : IntegrationEvent;
