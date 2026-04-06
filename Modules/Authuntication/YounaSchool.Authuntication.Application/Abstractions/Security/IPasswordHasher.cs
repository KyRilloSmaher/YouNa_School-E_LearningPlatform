namespace YounaSchool.Authuntication.Application.Abstractions.Security;

/// <summary>
/// Abstraction over password hashing implementation (e.g., BCrypt).
/// </summary>
public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string passwordHash);
}

