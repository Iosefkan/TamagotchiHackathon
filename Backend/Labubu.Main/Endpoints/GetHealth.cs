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

public class GetHealth : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapGet(ApiRoutes.Labubu.Health, Handle)
        .RequireAuthorization()
        .WithSummary("Get Labubu health status");

    private record Response(int health, int weight, int status, string? message);

    private static async Task<Results<Ok<Response>, NotFound, BadRequest<string>>> Handle(
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

        var result = await labubuService.GetHealthAsync(labubu.Id);

        if (result.IsFailed)
            return TypedResults.NotFound();

        var dto = result.Value;
        var response = new Response(
            dto.Health,
            dto.Weight,
            dto.Status,
            dto.Message
        );

        return TypedResults.Ok(response);
    }
}
