using FluentResults;
using Labubu.Main.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using Labubu.Main.DAL;

namespace Labubu.Main.Services;

public class AchievementService : IAchievementService
{
    private readonly MainContext _context;
    private readonly IMinioService _minioService;
    private readonly IDistributedCache _cache;
    private const int CacheExpirationMinutes = 30;

    public AchievementService(
        MainContext context, 
        IMinioService minioService,
        IDistributedCache cache)
    {
        _context = context;
        _minioService = minioService;
        _cache = cache;
    }

    public async Task<Result<List<AchievementDto>>> GetAchievementsAsync(Guid labubuId)
    {
        const string achievementsCacheKey = "achievements_list";
        
        List<Achievement>? achievements = null;
        var cachedAchievements = await _cache.GetStringAsync(achievementsCacheKey);
        
        if (!string.IsNullOrEmpty(cachedAchievements))
        {
            achievements = JsonSerializer.Deserialize<List<Achievement>>(cachedAchievements);
        }

        if (achievements is null)
        {
            achievements = await _context.Achievements
                .Include(a => a.AchievementProgresses)
                .ToListAsync();

            try
            {
                var jsonData = JsonSerializer.Serialize(achievements);
                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CacheExpirationMinutes)
                };
                await _cache.SetStringAsync(achievementsCacheKey, jsonData, cacheOptions);
            }
            catch
            {
            }
        }

        var labubuProgress = await _context.AchievementProgresses
            .Where(ap => ap.LabubuId == labubuId)
            .ToDictionaryAsync(ap => ap.AchievementId, ap => ap.IsCompleted);

        var urlTasks = achievements.Select(async achievement =>
        {
            var urlResult = await _minioService.GetFileUrlAsync(achievement.Url);
            var url = urlResult.IsSuccess ? urlResult.Value : achievement.Url;
            return new AchievementDto(
                achievement.Id,
                achievement.Name,
                achievement.Description,
                url,
                labubuProgress.GetValueOrDefault(achievement.Id, false)
            );
        });

        var result = await Task.WhenAll(urlTasks);

        return Result.Ok(result.ToList());
    }
}
