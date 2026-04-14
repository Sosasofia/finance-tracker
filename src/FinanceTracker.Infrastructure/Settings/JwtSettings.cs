namespace FinanceTracker.Infrastructure.Settings;

public class JwtSettings
{
    public string Key { get; init; } = string.Empty;
    public string Issuer { get; init; } = string.Empty;
    public int ExpiryMinutes { get; init; }
}
