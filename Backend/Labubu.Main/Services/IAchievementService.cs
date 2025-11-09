using FluentResults;
using Labubu.Main.DAL;

namespace Labubu.Main.Services;

public interface IAchievementService
{
    Task<Result<List<AchievementDto>>> GetAchievementsAsync(Guid labubuId);
}

public record AchievementDto(Guid AchievementId, string Name, string Description, string Url, bool IsCompleted);

