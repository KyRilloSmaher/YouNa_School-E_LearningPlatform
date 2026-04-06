
using SharedKernel.Domain.CoreAbstractions;
using Shared.Application.RESULT_PATTERN;
using System.Net;

namespace YounaSchool.Authentication.Domain.ValueObjects;

public sealed class RefreshToken : ValueObject
{
    public string TokenHash { get; }
    public DateTime ExpiresAtUtc { get; }

    public RefreshToken() { }
    private RefreshToken(string tokenHash, DateTime expiresAtUtc)
    {
        TokenHash = tokenHash;
        ExpiresAtUtc = expiresAtUtc;
    }

    /// <summary>
    /// Creates a new <see cref="RefreshToken"/> value object with validation.
    /// </summary>
    /// <param name="tokenHash">Hashed representation of the refresh token.</param>
    /// <param name="expiresAtUtc">UTC expiration timestamp.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> containing the created <see cref="RefreshToken"/> when valid,
    /// otherwise a failure result with the validation error.
    /// </returns>
    public static Result<RefreshToken> Create(string tokenHash, DateTime expiresAtUtc)
    {
        if (string.IsNullOrWhiteSpace(tokenHash))
            return Result<RefreshToken>.Failure("Token hash cannot be empty.", HttpStatusCode.BadRequest);

        if (expiresAtUtc <= DateTime.UtcNow)
            return Result<RefreshToken>.Failure("Expiration must be in the future.", HttpStatusCode.BadRequest);

        var value = new RefreshToken(tokenHash, expiresAtUtc);
        return Result<RefreshToken>.Success(value);
    }

    public bool IsExpired() => DateTime.UtcNow >= ExpiresAtUtc;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return TokenHash;
        yield return ExpiresAtUtc;
    }
}