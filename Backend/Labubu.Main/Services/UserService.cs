using FluentResults;
using Labubu.Main.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Shared.Contracts;
using System.Text.Json;

namespace Labubu.Main.Services;

public class UserService : IUserService
{
    private readonly MainContext _context;
    private readonly ITransactionService _transactionService;
    private readonly IDistributedCache _cache;
    private const int UserInfoCacheMinutes = 5;
    private const int UserEnergyCacheMinutes = 2;

    public UserService(
        MainContext context,
        ITransactionService transactionService,
        IDistributedCache cache)
    {
        _context = context;
        _transactionService = transactionService;
        _cache = cache;
    }

    public async Task<Result<UserEnergyDto>> GetEnergyAsync(Guid userId)
    {
        var cacheKey = $"user_energy_{userId}";
        
        var cachedData = await _cache.GetStringAsync(cacheKey);
        if (!string.IsNullOrEmpty(cachedData))
        {
            try
            {
                var cachedResult = JsonSerializer.Deserialize<UserEnergyDto>(cachedData);
                if (cachedResult != null)
                {
                    return Result.Ok(cachedResult);
                }
            }
            catch
            {
            }
        }

        var labubu = await _context.Labubus
            .FirstOrDefaultAsync(l => l.UserId == userId);

        if (labubu == null)
            return Result.Fail("Labubu not found for user");

        var energyDto = new UserEnergyDto(labubu.Energy);

        try
        {
            var jsonData = JsonSerializer.Serialize(energyDto);
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(UserEnergyCacheMinutes)
            };
            await _cache.SetStringAsync(cacheKey, jsonData, cacheOptions);
        }
        catch
        {
        }

        return Result.Ok(energyDto);
    }

    public async Task<Result<UserInfoDto>> GetUserInfoAsync(Guid userId)
    {
        var cacheKey = $"user_info_{userId}";
        
        var cachedData = await _cache.GetStringAsync(cacheKey);
        if (!string.IsNullOrEmpty(cachedData))
        {
            try
            {
                var cachedResult = JsonSerializer.Deserialize<UserInfoDto>(cachedData);
                if (cachedResult != null)
                {
                    return Result.Ok(cachedResult);
                }
            }
            catch
            {
            }
        }

        var user = await _context.Users
            .Include(u => u.Labubu)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            return Result.Fail("User not found");

        var fromDate = DateTimeOffset.UtcNow.AddMonths(-1);
        var toDate = DateTimeOffset.UtcNow;
        
        var statisticsResult = await _transactionService.GetUserStatisticsAsync(
            user.Name,
            fromDate,
            toDate);

        var banks = new List<BankInfoDto>();
        if (statisticsResult.IsSuccess)
        {
            var bankInfoResult = await _transactionService.GetBanksInfoAsync();
            if (bankInfoResult.IsSuccess)
            {
                var bankInfos = bankInfoResult.Value;
                int bankId = 1;
                foreach (var bankName in statisticsResult.Value.BankNames)
                {
                    var bankInfo = bankInfos.FirstOrDefault(b => b.BankName == bankName);
                    if (bankInfo != null)
                    {
                        banks.Add(new BankInfoDto(
                            bankId++,
                            bankName,
                            statisticsResult.Value.FundsAvailable / Math.Max(1, statisticsResult.Value.BankNames.Count)
                        ));
                    }
                }
            }
        }

        var userInfoDto = new UserInfoDto(
            user.Name,
            user.Email,
            banks
        );

        try
        {
            var jsonData = JsonSerializer.Serialize(userInfoDto);
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(UserInfoCacheMinutes)
            };
            await _cache.SetStringAsync(cacheKey, jsonData, cacheOptions);
        }
        catch
        {
        }

        return Result.Ok(userInfoDto);
    }

    public async Task InvalidateUserCacheAsync(Guid userId)
    {
        try
        {
            await _cache.RemoveAsync($"user_info_{userId}");
            await _cache.RemoveAsync($"user_energy_{userId}");
        }
        catch
        {
        }
    }
}
