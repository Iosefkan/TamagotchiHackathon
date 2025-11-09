using FluentResults;
using Labubu.Main.DAL;
using Labubu.Main.DAL.Enums;
using Labubu.Main.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace Labubu.Main.Services;

public class LabubuService : ILabubuService
{
    private readonly MainContext _context;
    private readonly IMinioService _minioService;
    private readonly IBattlePassService _battlePassService;
    private readonly IDistributedCache _cache;

    public LabubuService(
        MainContext context,
        IMinioService minioService,
        IBattlePassService battlePassService,
        IDistributedCache cache)
    {
        _context = context;
        _minioService = minioService;
        _battlePassService = battlePassService;
        _cache = cache;
    }

    public async Task<Result<LabubuHealthDto>> GetHealthAsync(Guid labubuId)
    {
        var labubu = await _context.Labubus.FindAsync(labubuId);
        if (labubu == null)
            return Result.Fail("Labubu not found");

        return Result.Ok(new LabubuHealthDto(
            labubu.Health,
            labubu.Weight,
            labubu.Status,
            labubu.Message
        ));
    }

    public async Task<Result<List<CurrentClothesDto>>> GetCurrentClothesAsync(Guid labubuId)
    {
        var labubu = await _context.Labubus
            .Include(l => l.LabubuClothes)
                .ThenInclude(lc => lc.Clothes)
            .FirstOrDefaultAsync(l => l.Id == labubuId);

        if (labubu == null)
            return Result.Fail("Labubu not found");

        var urlTasks = labubu.LabubuClothes.Select(async labubuClothes =>
        {
            var urlResult = await _minioService.GetFileUrlAsync(labubuClothes.Clothes.Url);
            var url = urlResult.IsSuccess ? urlResult.Value : labubuClothes.Clothes.Url;
            return new CurrentClothesDto(
                (int)labubuClothes.Clothes.Type,
                labubuClothes.ClothesId,
                url
            );
        });

        var clothesList = await Task.WhenAll(urlTasks);

        return Result.Ok(clothesList.ToList());
    }

    public async Task<Result<LabubuActionDto>> PerformActionAsync(Guid labubuId, ActionType actionType)
    {
        var labubu = await _context.Labubus.FindAsync(labubuId);
        if (labubu == null)
            return Result.Fail("Labubu not found");

        switch (actionType)
        {
            case ActionType.Feed:
                labubu.Health = Math.Min(100, labubu.Health + 10);
                labubu.Weight = Math.Min(100, labubu.Weight + 1);
                labubu.Energy = Math.Max(0, labubu.Energy - 5);
                break;
            case ActionType.Play:
                labubu.Health = Math.Min(100, labubu.Health + 5);
                labubu.Energy = Math.Max(0, labubu.Energy - 15);
                break;
            case ActionType.Sleep:
                labubu.Health = Math.Min(100, labubu.Health + 20);
                labubu.Energy = Math.Min(100, labubu.Energy + 50);
                break;
            case ActionType.Exercise:
                labubu.Health = Math.Min(100, labubu.Health + 15);
                labubu.Weight = Math.Max(1, labubu.Weight - 1);
                labubu.Energy = Math.Max(0, labubu.Energy - 20);
                break;
            case ActionType.Clean:
                labubu.Health = Math.Min(100, labubu.Health + 10);
                labubu.Energy = Math.Max(0, labubu.Energy - 5);
                break;
            case ActionType.Heal:
                labubu.Health = Math.Min(100, labubu.Health + 30);
                labubu.Energy = Math.Max(0, labubu.Energy - 10);
                break;
            case ActionType.Work:
                labubu.Energy = Math.Max(0, labubu.Energy - 30);
                break;
            default:
                labubu.Energy = Math.Max(0, labubu.Energy - 10);
                break;
        }

        await _context.SaveChangesAsync();

        try
        {
            await _cache.RemoveAsync($"user_energy_{labubu.UserId}");
        }
        catch
        {
        }

        await _battlePassService.AddXpAsync(labubuId, 5, $"Action_{actionType}");

        if (labubu.Health < 30)
        {
            await _battlePassService.AddXpAsync(labubuId, 10, "LowHealthRecovery");
        }

        return Result.Ok(new LabubuActionDto(
            labubu.Health,
            labubu.Weight,
            labubu.Status
        ));
    }
}
