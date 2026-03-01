namespace Bintainer.Modules.Users.Infrastructure.Identity;

internal sealed class JwtOptions
{
    public const string SectionName = "Users:Jwt";

    public string Issuer { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    public string Secret { get; init; } = string.Empty;
    public int ExpirationInMinutes { get; init; } = 60;
}
