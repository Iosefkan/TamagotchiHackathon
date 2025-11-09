using Common;
using Common.Extensions;
using Labubu.Main.Extensions;
using Labubu.Main.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Labubu.Main.Endpoints;

public class GetUserEnergy : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapGet(ApiRoutes.Users.Energy, Handle)
        .RequireAuthorization()
        .WithSummary("Get user energy");

    private record Response(int energy);

    private static async Task<Results<Ok<Response>, NotFound, BadRequest<string>>> Handle(
        ClaimsPrincipal user,
        IUserService userService)
    {
        var userId = user.GetUserId();
        if (!userId.HasValue)
            return TypedResults.BadRequest("Invalid user");

        var result = await userService.GetEnergyAsync(userId.Value);

        if (result.IsFailed)
            return TypedResults.NotFound();

        var dto = result.Value;
        var response = new Response(dto.Energy);

        return TypedResults.Ok(response);
    }
}

