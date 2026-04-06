using Shared.Application.IntegrationEvents;

namespace YounaSchool.Authuntication.Application.IntegrationEvents;

public sealed record UserRegisteredIntegrationEvent : IntegrationEvent
{
    public Guid UserId { get; init; }
    public string Email { get; init; } = null!;
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string Role { get; init; } = null!;

    public UserRegisteredIntegrationEvent(
        Guid userId,
        string email,
        string firstName,
        string lastName,
        string role)
    {
        UserId = userId;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        Role = role;
    }

    public UserRegisteredIntegrationEvent() { }
}
