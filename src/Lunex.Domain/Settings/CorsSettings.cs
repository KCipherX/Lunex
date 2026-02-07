namespace Lunex.Domain.Settings;

public sealed class CorsSettings
{
    public const string SectionName = "CorsSettings";

    public const string PolicyName = "lunex-policy";

    public required string[] AllowedOrigins { get; set; }
}
