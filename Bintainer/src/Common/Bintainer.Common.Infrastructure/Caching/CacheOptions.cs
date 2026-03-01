namespace Bintainer.Common.Infrastructure.Caching;

public sealed class CacheOptions
{
    public TimeSpan DefaultExpiration { get; init; } = TimeSpan.FromMinutes(5);
}
