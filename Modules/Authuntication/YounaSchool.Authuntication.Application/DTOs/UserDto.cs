namespace YounaSchool.Authuntication.Application.DTOs;

public sealed class UserDto
{
    public Guid Id { get; init; }
    public string Email { get; init; } = null!;
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public IReadOnlyCollection<string> Roles { get; init; } = Array.Empty<string>();
}

