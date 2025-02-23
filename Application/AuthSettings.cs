namespace Application;

public class AuthSettings
{
    public TimeSpan AccessTokenLifetime { get; set; }
    public TimeSpan RefreshTokenLifetime { get; set; }
    public string SecretKey { get; set; } = string.Empty;
}