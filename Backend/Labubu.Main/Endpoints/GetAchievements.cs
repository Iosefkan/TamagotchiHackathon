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

public class GetAchievements : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapGet(ApiRoutes.Achievements.List, Handle)
        .RequireAuthorization()
        .WithSummary("Get achievements for Labubu");

    private record ItemResponse(Guid achievementId, string name, string desc, string url, bool isCompleted);

    private static async Task<Results<Ok<List<ItemResponse>>, NotFound, BadRequest<string>>> Handle(
        ClaimsPrincipal user,
        IAchievementService achievementService,
        MainContext context)
    {
        var userId = user.GetUserId();
        if (!userId.HasValue)
            return TypedResults.BadRequest("Invalid user");

        var labubu = await context.Labubus.FirstOrDefaultAsync(l => l.UserId == userId.Value);
        if (labubu == null)
            return TypedResults.NotFound();

        var result = await achievementService.GetAchievementsAsync(labubu.Id);
        
        var response = result.Value.Select(dto => new ItemResponse(
            dto.AchievementId,
            dto.Name,
            dto.Description,
            dto.Url,
            dto.IsCompleted
        )).ToList();

        return TypedResults.Ok(response);
    }
}