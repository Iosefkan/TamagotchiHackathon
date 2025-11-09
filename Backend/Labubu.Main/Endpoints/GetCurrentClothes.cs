using Common;
using Common.Extensions;
using Labubu.Main.Data;
using Labubu.Main.Extensions;
using Labubu.Main.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Labubu.Main.Endpoints;

public class GetCurrentClothes : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapGet(ApiRoutes.Labubu.ClothesCurrent, Handle)
        .RequireAuthorization()
        .WithSummary("Get current clothes for Labubu");

    private record ItemResponse(int bodyPartId, Guid clothId, string url);

    private static async Task<Results<Ok<List<ItemResponse>>, NotFound, BadRequest<string>>> Handle(
        ClaimsPrincipal user,
        ILabubuService labubuService,
        MainContext context)
    {
        var userId = user.GetUserId();
        if (!userId.HasValue)
            return TypedResults.BadRequest("Invalid user");

        var labubu = await context.Labubus.FirstOrDefaultAsync(l => l.UserId == userId.Value);
        if (labubu == null)
            return TypedResults.NotFound();

        var result = await labubuService.GetCurrentClothesAsync(labubu.Id);

        if (result.IsFailed)
            return TypedResults.NotFound();

        var response = result.Value.Select(dto => new ItemResponse(
            dto.BodyPartId,
            dto.ClothId,
            dto.Url
        )).ToList();

        return TypedResults.Ok(response);
    }
}

