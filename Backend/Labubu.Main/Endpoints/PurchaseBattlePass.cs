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

public class PurchaseBattlePass : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost(ApiRoutes.BattlePasses.Purchase, Handle)
        .RequireAuthorization()
        .WithSummary("Purchase battle pass");

    private static async Task<Results<Ok, NotFound, BadRequest<string>>> Handle(
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

        var result = await battlePassService.PurchaseBattlePassAsync(battlePassId, labubu.Id);
        
        if (result.IsFailed)
            return TypedResults.BadRequest(result.Errors.First().Message);

        return TypedResults.Ok();
    }
}
