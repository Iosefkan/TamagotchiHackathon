using FluentResults;
using Labubu.Main.DAL;
using Labubu.Main.DAL.Enums;

namespace Labubu.Main.Services;

public interface IClothesService
{
    Task<Result> SetClothesAsync(Guid labubuId, Guid clothId);
    Task<Result<List<ClothesDto>>> GetClothesAsync(ClothesType? bodyPartId);
}

public record ClothesDto(Guid ClothesId, string Name, string Url);