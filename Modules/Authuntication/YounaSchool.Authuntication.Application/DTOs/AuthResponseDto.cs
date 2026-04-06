namespace YounaSchool.Authuntication.Application.DTOs;

/// <summary>
/// Represents the response returned after a successful authentication operation.
/// </summary>
public sealed class AuthResponseDto
{
    public string? AccessToken { get; init; }
    public string? RefreshToken { get; init; }
    public Guid? SessionId { get; init; }
    public string? Message { get; init; }
}

