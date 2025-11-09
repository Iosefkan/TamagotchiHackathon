namespace Labubu.Auth.Configuration;

public class JwtConfig
{
    public string SecretKey { get; set; } = null!;
    public string Issuer { get; set; } = "LabubuAuth";
    public string Audience { get; set; } = "LabubuApp";
    public int AccessTokenExpirationMinutes { get; set; } = 60;
    public int RefreshTokenExpirationDays { get; set; } = 30;
}

