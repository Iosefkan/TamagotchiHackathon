using FluentResults;
using Labubu.Main.DAL;
using Labubu.Main.DAL.Enums;

namespace Labubu.Main.Services;

public interface ILabubuService
{
    Task<Result<LabubuHealthDto>> GetHealthAsync(Guid labubuId);
    Task<Result<List<CurrentClothesDto>>> GetCurrentClothesAsync(Guid labubuId);
    Task<Result<LabubuActionDto>> PerformActionAsync(Guid labubuId, ActionType actionType);
}

public record LabubuHealthDto(int Health, int Weight, int Status, string? Message);
public record CurrentClothesDto(int BodyPartId, Guid ClothId, string Url);
public record LabubuActionDto(int Health, int Weight, int Status);

