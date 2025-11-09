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
using System.Text.Json;

namespace Labubu.Main.Endpoints;

public class GameEnd : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost(ApiRoutes.Games.End, Handle)
        .RequireAuthorization()
        .WithSummary("End a game session")
        .WithRequestValidation<Request>();

    private record Request(int game_id, int score, JsonDocument? specific_data);

    private class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.game_id).GreaterThan(0);
            RuleFor(x => x.score).GreaterThanOrEqualTo(0);
        }
    }

    private record Response(int? award);

    private static async Task<Results<Ok<Response>, NotFound, BadRequest<string>>> Handle(
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

        var result = await gameService.EndGameAsync(labubu.Id, request.game_id, request.score, request.specific_data);

        if (result.IsFailed)
            return TypedResults.NotFound();

        var dto = result.Value;
        var response = new Response(dto.Award);

        return TypedResults.Ok(response);
    }
}

