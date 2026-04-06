namespace YounaSchool.Authuntication.Application.Abstractions.Security;

/// <summary>
/// Provides operations for managing and verifying two-factor authentication (TOTP-based).
/// </summary>
public interface ITwoFactorService
{
    /// <summary>
    /// Generates a new secret and corresponding QR code URI for the specified user.
    /// </summary>
    Task<(string secretKey, string qrCodeUri)> GenerateSetupInfoAsync(Guid userId, string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates a TOTP code against the provided secret key.
    /// </summary>
    bool VerifyCode(string secretKey, string code);
}

