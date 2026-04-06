using Shared.Application.RESULT_PATTERN;
using SharedKernel.Domain.CoreAbstractions;
using YounaSchool.Authentication.Domain.Enums;
using YounaSchool.Authentication.Domain.ValueObjects;
using YounaSchool.Authuntication.Domain.Events;

namespace YounaSchool.Authuntication.Domain.Aggregates;

public sealed class AuthSession : AggregateRoot
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }

    public RefreshToken RefreshToken { get; private set; } = null!;
    public SessionStatus Status { get; private set; }

    public string? DeviceInfo { get; private set; }
    public string? IpAddress { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? RevokedAtUtc { get; private set; }

    private AuthSession() { }

    private AuthSession(
        Guid id,
        Guid userId,
        RefreshToken refreshToken,
        string? deviceInfo,
        string? ipAddress)
    {
        Id = id;
        UserId = userId;
        RefreshToken = refreshToken;
        DeviceInfo = deviceInfo;
        IpAddress = ipAddress;
        Status = SessionStatus.Active;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public static Result<AuthSession> Create(
        Guid userId,
        RefreshToken refreshToken,
        string? deviceInfo,
        string? ipAddress)
    {
        if (userId == Guid.Empty)
        {
            return Result<AuthSession>.Failure("UserId cannot be empty.", System.Net.HttpStatusCode.BadRequest);
        }

        if (refreshToken is null)
        {
            return Result<AuthSession>.Failure("Refresh token is required.", System.Net.HttpStatusCode.BadRequest);
        }

        if (refreshToken.IsExpired())
        {
            return Result<AuthSession>.Failure("Cannot create session with expired refresh token.", System.Net.HttpStatusCode.BadRequest);
        }

        var session = new AuthSession(
            Guid.NewGuid(),
            userId,
            refreshToken,
            deviceInfo,
            ipAddress);

        session.RaiseDomainEvent(new UserLoggedInDomainEvent(userId));

        return Result<AuthSession>.Success(session);
    }

    public Result<bool> Revoke(string reason)
    {
        if (Status != SessionStatus.Active)
            return Result<bool>.Failure("Session already closed.", System.Net.HttpStatusCode.BadRequest);

        Status = SessionStatus.Revoked;
        RevokedAtUtc = DateTime.UtcNow;

        RaiseDomainEvent(new SessionRevokedDomainEvent(Id, reason));

        return Result<bool>.Success(true);
    }

    public void MarkExpired()
    {
        if (Status == SessionStatus.Active && RefreshToken.IsExpired())
        {
            Status = SessionStatus.Expired;
            RaiseDomainEvent(new SessionExpiredDomainEvent(Id));
        }
    }
}