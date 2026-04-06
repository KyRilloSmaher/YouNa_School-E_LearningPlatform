using SharedKernel.Domain.CoreAbstractions;
using Shared.Application.RESULT_PATTERN;
using System.Net;

namespace YounaSchool.Authentication.Domain.ValueObjects;

/// <summary>
/// Represents a secret used for two-factor authentication (e.g., TOTP).
/// </summary>
public sealed class TwoFactorSecret : ValueObject
{
    public string SecretKey { get; }
    public bool IsEnabled { get; private set; }
    public DateTime CreatedAtUtc { get; }
    public DateTime? DisabledAtUtc { get; private set; }

    public TwoFactorSecret() { }
    private TwoFactorSecret(string secretKey, bool isEnabled)
    {
        SecretKey = secretKey;
        IsEnabled = isEnabled;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public static Result<TwoFactorSecret> Create(string secretKey, bool enabled = false)
    {
        if (string.IsNullOrWhiteSpace(secretKey))
        {
            return Result<TwoFactorSecret>.Failure("Two-factor secret key is required.", HttpStatusCode.BadRequest);
        }

        return Result<TwoFactorSecret>.Success(new TwoFactorSecret(secretKey, enabled));
    }

    public void Enable()
    {
        IsEnabled = true;
        DisabledAtUtc = null;
    }

    public void Disable()
    {
        IsEnabled = false;
        DisabledAtUtc = DateTime.UtcNow;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return SecretKey;
    }
}

