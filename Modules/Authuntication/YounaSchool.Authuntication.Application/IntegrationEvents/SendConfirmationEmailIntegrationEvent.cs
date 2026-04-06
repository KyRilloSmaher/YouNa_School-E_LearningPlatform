using Shared.Application.IntegrationEvents;

namespace YounaSchool.Authuntication.Application.IntegrationEvents;

public sealed record SendConfirmationEmailIntegrationEvent(
    Guid UserId,
    string Email,
    string Title,
    string Message,
    string ConfirmationLink
) : IntegrationEvent;
