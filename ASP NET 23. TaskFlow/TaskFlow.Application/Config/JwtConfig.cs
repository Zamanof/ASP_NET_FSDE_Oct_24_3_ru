namespace TaskFlow.Application.Config;

public class JwtConfig
{
    public const string SectionName = "JWTSettings";

    public string SecretKey { get; set; } = string.Empty;
    public string RefreshTokenSecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpirationInMinutes { get; set; } = 15;
    public int RefreshTokenExpirationInDays { get; set; } = 7;
}
