namespace YounaSchool.Authuntication.Infrastructure.Security;

public sealed class JwtSettings
{
    public const string SectionName = "Jwt";

    public string Secret { get; set; } = null!;
    public string Issuer { get; set; } = "YouNaSchool";
    public string Audience { get; set; } = "YouNaSchool";
    public int ExpirationMinutes { get; set; } = 60;
}
