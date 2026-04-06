using System.Security.Claims;

namespace YounaSchool.Authuntication.Application.Abstractions.Security;

/// <summary>
/// Generates JWT access tokens and refresh tokens for authenticated users.
/// </summary>
public interface ITokenGenerator
{
    string GenerateAccessToken(IEnumerable<Claim> claims);

    /// <summary>
    /// Generates a raw refresh token string (will be hashed before persistence).
    /// </summary>
    string GenerateRefreshToken();
}

