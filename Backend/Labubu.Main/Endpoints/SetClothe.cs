using Common;
using Common.Extensions;
using FluentResults;
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

public class SetClothe : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost(ApiRoutes.Labubu.Clothes, Handle)
        .RequireAuthorization()
        .WithSummary("Set clothes for Labubu")
        .WithRequestValidation<Request>();

    private record Request(int body_part_id, Guid cloth_id);

    private class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.body_part_id).GreaterThan(0);
            RuleFor(x => x.cloth_id).NotEmpty();
        }
    }

    private static async Task<Results<Ok, NotFound, BadRequest<string>>> Handle(
        [FromBody] Request request,
        ClaimsPrincipal user,
        IClothesService clothesService,
        MainContext context)
    {
        var userId = user.GetUserId();
        if (!userId.HasValue)
            return TypedResults.BadRequest("Invalid user");

        var labubu = await context.Labubus.FirstOrDefaultAsync(l => l.UserId == userId.Value);
        if (labubu == null)
            return TypedResults.NotFound();

        var result = await clothesService.SetClothesAsync(labubu.Id, request.cloth_id);

        if(result.IsFailed)
            return TypedResults.NotFound();

        return TypedResults.Ok();
    }
}