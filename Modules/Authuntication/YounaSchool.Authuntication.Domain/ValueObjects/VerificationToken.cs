using SharedKernel.Domain.CoreAbstractions;
using Shared.Application.RESULT_PATTERN;
using System.Net;

namespace YounaSchool.Authentication.Domain.ValueObjects;

/// <summary>
/// Represents a one-time verification token (e.g., for email verification or password reset).
/// </summary>
public sealed class VerificationToken : ValueObject
{
    public string Token { get; }
    public DateTime ExpiresAtUtc { get; }
    public bool IsUsed { get; private set; }
    public DateTime? UsedAtUtc { get; private set; }

    public VerificationToken() { }
    private VerificationToken(string token, DateTime expiresAtUtc)
    {
        Token = token;
        ExpiresAtUtc = expiresAtUtc;
        IsUsed = false;
    }

    public static Result<VerificationToken> Create(string token, DateTime expiresAtUtc)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return Result<VerificationToken>.Failure("Verification token cannot be empty.", HttpStatusCode.BadRequest);
        }

        if (expiresAtUtc <= DateTime.UtcNow)
        {
            return Result<VerificationToken>.Failure("Verification token expiration must be in the future.", HttpStatusCode.BadRequest);
        }

        return Result<VerificationToken>.Success(new VerificationToken(token, expiresAtUtc));
    }

    public bool IsExpired() => DateTime.UtcNow >= ExpiresAtUtc;

    public Result<bool> MarkUsed()
    {
        if (IsUsed)
        {
            return Result<bool>.Failure("Verification token already used.", HttpStatusCode.BadRequest);
        }

        if (IsExpired())
        {
            return Result<bool>.Failure("Verification token has expired.", HttpStatusCode.BadRequest);
        }

        IsUsed = true;
        UsedAtUtc = DateTime.UtcNow;

        return Result<bool>.Success(true);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Token;
        yield return ExpiresAtUtc;
    }
}

