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

public class GetBattlePassInfo : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapGet(ApiRoutes.BattlePasses.Info, Handle)
        .RequireAuthorization()
        .WithSummary("Get battle pass information");

    private static async Task<Results<Ok<Services.BattlePassInfoDto>, NotFound, BadRequest<string>>> Handle(
        [FromRoute] Guid battlePassId,
        ClaimsPrincipal user,
        IBattlePassService battlePassService,
        MainContext context)
    {
        var userId = user.GetUserId();
        if (!userId.HasValue)
            return TypedResults.BadRequest("Invalid user");

        var labubu = await context.Labubus.FirstOrDefaultAsync(l => l.UserId == userId.Value);
        if (labubu == null)
            return TypedResults.NotFound();
        
        var result = await battlePassService.GetBattlePassInfoAsync(battlePassId, labubu.Id);
        
        if (result.IsFailed)
            return TypedResults.NotFound();

        return TypedResults.Ok(result.Value);
    }
}
