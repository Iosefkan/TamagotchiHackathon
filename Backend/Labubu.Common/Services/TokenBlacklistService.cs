using Microsoft.Extensions.Caching.Distributed;

namespace Common.Services;

public class TokenBlacklistService : ITokenBlacklistService
{
    private const string CachePrefix = "jwt:blacklist:";
    private readonly IDistributedCache _cache;

    public TokenBlacklistService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task BlacklistTokenAsync(string jti, TimeSpan expiration)
    {
        await _cache.SetStringAsync(
            BuildCacheKey(jti),
            "blacklisted",
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration
            });
    }

    public async Task<bool> IsTokenBlacklistedAsync(string jti)
    {
        var value = await _cache.GetStringAsync(BuildCacheKey(jti));
        return value is not null;
    }

    private static string BuildCacheKey(string jti) => $"{CachePrefix}{jti}";
}