using Common;
using Common.Extensions;
using Labubu.Main.Extensions;
using Labubu.Main.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Labubu.Main.Endpoints;

public class GetActiveBattlePasses : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapGet(ApiRoutes.BattlePasses.Active, Handle)
        .RequireAuthorization()
        .WithSummary("Get active battle passes");

    private static async Task<Ok<List<Services.BattlePassInfoDto>>> Handle(
        IBattlePassService battlePassService)
    {
        var result = await battlePassService.GetActiveBattlePassesAsync();
        return TypedResults.Ok(result.Value);
    }
}
