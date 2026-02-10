namespace Lunex.Application.Common.Configurations;

public sealed class JwtSettings
{
    public const string Section = nameof(JwtSettings);
    
    public string SecretKey { get; init; } = null!;
    public string Issuer { get; init; } = null!;
    public string Audience { get; init; } = null!;
    public int ExpiryMinutes { get; init; }
}