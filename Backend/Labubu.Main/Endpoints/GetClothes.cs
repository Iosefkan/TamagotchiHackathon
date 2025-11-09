using Common;
using Common.Extensions;
using Labubu.Main.DAL.Enums;
using Labubu.Main.Extensions;
using Labubu.Main.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Labubu.Main.Endpoints;

public class GetClothes : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapGet(ApiRoutes.Clothes.List, Handle)
        .RequireAuthorization()
        .WithSummary("Get clothes list");

    private record ItemResponse(Guid clothesId, string name, string url);

    private static async Task<Ok<List<ItemResponse>>> Handle(
        [FromQuery] int? body_part_id,
        IClothesService clothesService)
    {
        ClothesType? type = body_part_id.HasValue ? (ClothesType?)body_part_id.Value : null;
        var result = await clothesService.GetClothesAsync(type);
        
        var response = result.Value.Select(dto => new ItemResponse(
            dto.ClothesId,
            dto.Name,
            dto.Url
        )).ToList();

        return TypedResults.Ok(response);
    }
}