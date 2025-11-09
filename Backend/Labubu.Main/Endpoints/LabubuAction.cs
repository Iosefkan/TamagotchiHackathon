using Common;
using Common.Extensions;
using FluentValidation;
using Labubu.Main.Data;
using Labubu.Main.DAL.Enums;
using Labubu.Main.Extensions;
using Labubu.Main.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Labubu.Main.Endpoints;

public class LabubuAction : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost(ApiRoutes.Labubu.Actions, Handle)
        .RequireAuthorization()
        .WithSummary("Perform action on Labubu")
        .WithRequestValidation<Request>();

    private record Request(int action_type);

    private class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.action_type).GreaterThan(0);
        }
    }

    private record Response(int health, int weight, int status);

    private static async Task<Results<Ok<Response>, NotFound, BadRequest<string>>> Handle(
        [FromBody] Request request,
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

        var actionType = (ActionType)request.action_type;
        var result = await labubuService.PerformActionAsync(labubu.Id, actionType);

        if (result.IsFailed)
            return TypedResults.NotFound();

        var dto = result.Value;
        var response = new Response(
            dto.Health,
            dto.Weight,
            dto.Status
        );

        return TypedResults.Ok(response);
    }
}

