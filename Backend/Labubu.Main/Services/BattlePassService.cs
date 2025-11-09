using FluentResults;
using Labubu.Main.DAL;
using Labubu.Main.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Labubu.Main.Services;

public class BattlePassService : IBattlePassService
{
    private readonly MainContext _context;
    private readonly IClothesService _clothesService;
    private readonly IDistributedCache _cache;
    private const int ActiveBattlePassesCacheMinutes = 10;
    private const int BattlePassInfoCacheMinutes = 30;
    private const int BattlePassRewardsCacheMinutes = 30;

    public BattlePassService(
        MainContext context,
        IClothesService clothesService,
        IDistributedCache cache)
    {
        _context = context;
        _clothesService = clothesService;
        _cache = cache;
    }

    public async Task<Result> AddXpAsync(Guid labubuId, int xp, string? source = null)
    {
        try
        {
            var activeBattlePasses = await _context.BattlePasses
                .Where(bp => bp.IsActive && bp.StartDate <= DateTime.UtcNow && bp.EndDate >= DateTime.UtcNow)
                .ToListAsync();

            foreach (var battlePass in activeBattlePasses)
            {
                var progress = await _context.BattlePassProgresses
                    .FirstOrDefaultAsync(p => p.BattlePassId == battlePass.Id && p.LabubuId == labubuId);

                if (progress == null)
                {
                    progress = new BattlePassProgress
                    {
                        Id = Guid.NewGuid(),
                        BattlePassId = battlePass.Id,
                        LabubuId = labubuId,
                        CurrentLevel = 1,
                        CurrentXp = 0,
                        LastUpdatedAt = DateTime.UtcNow
                    };
                    _context.BattlePassProgresses.Add(progress);
                }

                progress.CurrentXp += xp;
                progress.LastUpdatedAt = DateTime.UtcNow;

                while (progress.CurrentLevel < battlePass.MaxLevel &&
                       progress.CurrentXp >= battlePass.XpPerLevel * progress.CurrentLevel)
                {
                    progress.CurrentLevel++;
                }

                var maxXpForLevel = battlePass.XpPerLevel * progress.CurrentLevel;
                if (progress.CurrentLevel >= battlePass.MaxLevel)
                {
                    progress.CurrentXp = Math.Min(progress.CurrentXp, maxXpForLevel);
                }
            }

            await _context.SaveChangesAsync();
            await InvalidateBattlePassProgressCacheAsync(labubuId);
            
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail($"Failed to add XP: {ex.Message}");
        }
    }

    public async Task<Result<BattlePassInfoDto>> GetBattlePassInfoAsync(Guid battlePassId, Guid labubuId)
    {
        var baseInfoCacheKey = $"battlepass_info_{battlePassId}";
        
        BattlePass? battlePass = null;
        var cachedData = await _cache.GetStringAsync(baseInfoCacheKey);
        if (!string.IsNullOrEmpty(cachedData))
        {
            try
            {
                battlePass = JsonSerializer.Deserialize<BattlePass>(cachedData);
            }
            catch
            {
            }
        }

        if (battlePass == null)
        {
            battlePass = await _context.BattlePasses.FindAsync(battlePassId);
            if (battlePass == null)
                return Result.Fail("Battle pass not found");

            try
            {
                var jsonData = JsonSerializer.Serialize(battlePass);
                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(BattlePassInfoCacheMinutes)
                };
                await _cache.SetStringAsync(baseInfoCacheKey, jsonData, cacheOptions);
            }
            catch
            {
            }
        }

        var isPurchased = await _context.BattlePassPurchases
            .AnyAsync(p => p.BattlePassId == battlePassId && p.LabubuId == labubuId);

        var dto = new BattlePassInfoDto(
            battlePass.Id,
            battlePass.Name,
            battlePass.Description,
            battlePass.StartDate,
            battlePass.EndDate,
            battlePass.IsActive,
            battlePass.MaxLevel,
            battlePass.PurchasePrice,
            isPurchased);

        return Result.Ok(dto);
    }

    public async Task<Result<List<BattlePassInfoDto>>> GetActiveBattlePassesAsync()
    {
        const string cacheKey = "battlepass_active";
        
        var cachedData = await _cache.GetStringAsync(cacheKey);
        if (!string.IsNullOrEmpty(cachedData))
        {
            try
            {
                var cachedResult = JsonSerializer.Deserialize<List<BattlePassInfoDto>>(cachedData);
                if (cachedResult != null)
                {
                    return Result.Ok(cachedResult);
                }
            }
            catch
            {
            }
        }

        var battlePasses = await _context.BattlePasses
            .Where(bp => bp.IsActive && bp.StartDate <= DateTime.UtcNow && bp.EndDate >= DateTime.UtcNow)
            .ToListAsync();

        var dtos = battlePasses.Select(bp => new BattlePassInfoDto(
            bp.Id,
            bp.Name,
            bp.Description,
            bp.StartDate,
            bp.EndDate,
            bp.IsActive,
            bp.MaxLevel,
            bp.PurchasePrice,
            false)).ToList();

        try
        {
            var jsonData = JsonSerializer.Serialize(dtos);
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(ActiveBattlePassesCacheMinutes)
            };
            await _cache.SetStringAsync(cacheKey, jsonData, cacheOptions);
        }
        catch
        {
        }

        return Result.Ok(dtos);
    }

    public async Task<Result<BattlePassProgressDto>> GetBattlePassProgressAsync(Guid battlePassId, Guid labubuId)
    {
        var battlePass = await _context.BattlePasses.FindAsync(battlePassId);
        if (battlePass == null)
            return Result.Fail("Battle pass not found");

        var progress = await _context.BattlePassProgresses
            .FirstOrDefaultAsync(p => p.BattlePassId == battlePassId && p.LabubuId == labubuId);

        var isPurchased = await _context.BattlePassPurchases
            .AnyAsync(p => p.BattlePassId == battlePassId && p.LabubuId == labubuId);

        if (progress == null)
        {
            progress = new BattlePassProgress
            {
                Id = Guid.NewGuid(),
                BattlePassId = battlePassId,
                LabubuId = labubuId,
                CurrentLevel = 1,
                CurrentXp = 0,
                LastUpdatedAt = DateTime.UtcNow
            };
            _context.BattlePassProgresses.Add(progress);
            await _context.SaveChangesAsync();
        }

        var xpRequiredForNextLevel = progress.CurrentLevel < battlePass.MaxLevel
            ? battlePass.XpPerLevel * progress.CurrentLevel - progress.CurrentXp
            : 0;

        var dto = new BattlePassProgressDto(
            battlePassId,
            progress.CurrentLevel,
            progress.CurrentXp,
            xpRequiredForNextLevel,
            isPurchased);

        return Result.Ok(dto);
    }

    public async Task<Result> PurchaseBattlePassAsync(Guid battlePassId, Guid labubuId)
    {
        var battlePass = await _context.BattlePasses.FindAsync(battlePassId);
        if (battlePass == null)
            return Result.Fail("Battle pass not found");

        if (!battlePass.IsActive)
            return Result.Fail("Battle pass is not active");

        var existingPurchase = await _context.BattlePassPurchases
            .FirstOrDefaultAsync(p => p.BattlePassId == battlePassId && p.LabubuId == labubuId);

        if (existingPurchase != null)
            return Result.Fail("Battle pass already purchased");

        var purchase = new BattlePassPurchase
        {
            Id = Guid.NewGuid(),
            BattlePassId = battlePassId,
            LabubuId = labubuId,
            PurchasedAt = DateTime.UtcNow,
            PricePaid = battlePass.PurchasePrice
        };

        _context.BattlePassPurchases.Add(purchase);
        await _context.SaveChangesAsync();

        await InvalidateBattlePassCacheAsync(battlePassId, labubuId);

        return Result.Ok();
    }

    public async Task<Result> ClaimRewardAsync(Guid battlePassRewardId, Guid labubuId)
    {
        var reward = await _context.BattlePassRewards
            .Include(r => r.Clothes)
            .FirstOrDefaultAsync(r => r.Id == battlePassRewardId);

        if (reward == null)
            return Result.Fail("Reward not found");

        var existingClaim = await _context.BattlePassRewardClaims
            .FirstOrDefaultAsync(c => c.BattlePassRewardId == battlePassRewardId && c.LabubuId == labubuId);

        if (existingClaim != null)
            return Result.Fail("Reward already claimed");

        var progress = await _context.BattlePassProgresses
            .FirstOrDefaultAsync(p => p.BattlePassId == reward.BattlePassId && p.LabubuId == labubuId);

        if (progress == null || progress.CurrentLevel < reward.Level)
            return Result.Fail("Level requirement not met");

        if (reward.IsPremium)
        {
            var isPurchased = await _context.BattlePassPurchases
                .AnyAsync(p => p.BattlePassId == reward.BattlePassId && p.LabubuId == labubuId);

            if (!isPurchased)
                return Result.Fail("Premium battle pass required");
        }

        var claim = new BattlePassRewardClaim
        {
            Id = Guid.NewGuid(),
            BattlePassRewardId = battlePassRewardId,
            LabubuId = labubuId,
            ClaimedAt = DateTime.UtcNow
        };

        _context.BattlePassRewardClaims.Add(claim);

        if (reward.ClothesId.HasValue)
        {
            var labubu = await _context.Labubus.FindAsync(labubuId);
            if (labubu != null)
            {
                var labubuClothes = new LabubuClothes
                {
                    LabubuId = labubuId,
                    ClothesId = reward.ClothesId.Value
                };
                _context.LabubuClothes.Add(labubuClothes);
            }
        }

        await _context.SaveChangesAsync();
        
        await InvalidateBattlePassRewardsCacheAsync(reward.BattlePassId);
        await InvalidateBattlePassProgressCacheAsync(labubuId);
        
        return Result.Ok();
    }

    public async Task<Result<List<BattlePassRewardDto>>> GetAvailableRewardsAsync(Guid battlePassId, Guid labubuId)
    {
        var battlePass = await _context.BattlePasses.FindAsync(battlePassId);
        if (battlePass == null)
            return Result.Fail("Battle pass not found");

        var rewardsCacheKey = $"battlepass_rewards_{battlePassId}";
        
        List<BattlePassReward>? rewards = null;
        var cachedRewards = await _cache.GetStringAsync(rewardsCacheKey);
        if (!string.IsNullOrEmpty(cachedRewards))
        {
            try
            {
                rewards = JsonSerializer.Deserialize<List<BattlePassReward>>(cachedRewards);
            }
            catch
            {
            }
        }

        if (rewards == null)
        {
            rewards = await _context.BattlePassRewards
                .Include(r => r.Clothes)
                .Where(r => r.BattlePassId == battlePassId)
                .OrderBy(r => r.Level)
                .ToListAsync();

            try
            {
                var jsonData = JsonSerializer.Serialize(rewards);
                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(BattlePassRewardsCacheMinutes)
                };
                await _cache.SetStringAsync(rewardsCacheKey, jsonData, cacheOptions);
            }
            catch
            {
            }
        }

        var progress = await _context.BattlePassProgresses
            .FirstOrDefaultAsync(p => p.BattlePassId == battlePassId && p.LabubuId == labubuId);

        var currentLevel = progress?.CurrentLevel ?? 1;

        var isPurchased = await _context.BattlePassPurchases
            .AnyAsync(p => p.BattlePassId == battlePassId && p.LabubuId == labubuId);

        var claimedRewardIds = await _context.BattlePassRewardClaims
            .Where(c => c.LabubuId == labubuId && rewards.Select(r => r.Id).Contains(c.BattlePassRewardId))
            .Select(c => c.BattlePassRewardId)
            .ToListAsync();

        var rewardDtos = rewards.Select(r =>
        {
            var isClaimed = claimedRewardIds.Contains(r.Id);
            var canClaim = !isClaimed && currentLevel >= r.Level && (!r.IsPremium || isPurchased);

            return new BattlePassRewardDto(
                r.Id,
                r.Level,
                r.IsPremium,
                r.ClothesId,
                r.Clothes?.Name,
                r.Clothes?.Url,
                r.CurrencyAmount,
                r.CurrencyType,
                isClaimed,
                canClaim);
        }).ToList();

        return Result.Ok(rewardDtos);
    }

    private async Task InvalidateBattlePassCacheAsync(Guid battlePassId, Guid labubuId)
    {
        try
        {
            await _cache.RemoveAsync($"battlepass_info_{battlePassId}");
            await _cache.RemoveAsync("battlepass_active");
            await _cache.RemoveAsync($"battlepass_rewards_{battlePassId}");
        }
        catch
        {
        }
    }

    private async Task InvalidateBattlePassProgressCacheAsync(Guid labubuId)
    {
        try
        {
            await _cache.RemoveAsync("battlepass_active");
        }
        catch
        {
        }
    }

    private async Task InvalidateBattlePassRewardsCacheAsync(Guid battlePassId)
    {
        try
        {
            await _cache.RemoveAsync($"battlepass_rewards_{battlePassId}");
        }
        catch
        {
        }
    }
}
