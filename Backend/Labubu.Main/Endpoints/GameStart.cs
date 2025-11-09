using Common;
using Common.Extensions;
using FluentValidation;
using Labubu.Main.Data;
using Labubu.Main.Extensions;
using Labubu.Main.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Labubu.Main.Endpoints;

public class GameStart : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost(ApiRoutes.Games.Start, Handle)
        .RequireAuthorization()
        .WithSummary("Start a game session")
        .WithRequestValidation<Request>();

    private record Request(int game_id);

    private class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.game_id).GreaterThan(0);
        }
    }

    private static async Task<Results<Ok, NotFound, BadRequest<string>>> Handle(
        [FromBody] Request request,
        ClaimsPrincipal user,
        IGameService gameService,
        MainContext context)
    {
        var userId = user.GetUserId();
        if (!userId.HasValue)
            return TypedResults.BadRequest("Invalid user");

        var labubu = await context.Labubus.FirstOrDefaultAsync(l => l.UserId == userId.Value);
        if (labubu == null)
            return TypedResults.NotFound();

        var result = await gameService.StartGameAsync(labubu.Id, request.game_id);

        if (result.IsFailed)
            return TypedResults.NotFound();

        return TypedResults.Ok();
    }
}

