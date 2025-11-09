using FluentResults;

namespace Labubu.Main.Services;

public interface IBattlePassService
{
    Task<Result> AddXpAsync(Guid labubuId, int xp, string? source = null);
    Task<Result<BattlePassInfoDto>> GetBattlePassInfoAsync(Guid battlePassId, Guid labubuId);
    Task<Result<List<BattlePassInfoDto>>> GetActiveBattlePassesAsync();
    Task<Result<BattlePassProgressDto>> GetBattlePassProgressAsync(Guid battlePassId, Guid labubuId);
    Task<Result> PurchaseBattlePassAsync(Guid battlePassId, Guid labubuId);
    Task<Result> ClaimRewardAsync(Guid battlePassRewardId, Guid labubuId);
    Task<Result<List<BattlePassRewardDto>>> GetAvailableRewardsAsync(Guid battlePassId, Guid labubuId);
}

public record BattlePassInfoDto(
    Guid Id,
    string Name,
    string Description,
    DateTime StartDate,
    DateTime EndDate,
    bool IsActive,
    int MaxLevel,
    decimal PurchasePrice,
    bool IsPurchased);

public record BattlePassProgressDto(
    Guid BattlePassId,
    int CurrentLevel,
    int CurrentXp,
    int XpRequiredForNextLevel,
    bool IsPurchased);

public record BattlePassRewardDto(
    Guid Id,
    int Level,
    bool IsPremium,
    Guid? ClothesId,
    string? ClothesName,
    string? ClothesUrl,
    int? CurrencyAmount,
    string? CurrencyType,
    bool IsClaimed,
    bool CanClaim);
