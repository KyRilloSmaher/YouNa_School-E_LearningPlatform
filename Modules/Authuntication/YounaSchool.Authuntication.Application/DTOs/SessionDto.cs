namespace YounaSchool.Authuntication.Application.DTOs;

public sealed class SessionDto
{
    public Guid Id { get; init; }
    public string? DeviceInfo { get; init; }
    public string? IpAddress { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? RevokedAtUtc { get; init; }
    public string Status { get; init; } = null!;
}

