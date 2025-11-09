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

public class MarkMessageRead : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost(ApiRoutes.Labubu.MessagesRead, Handle)
        .RequireAuthorization()
        .WithSummary("Mark message as read");

    private static async Task<Results<Ok, NotFound, BadRequest<string>>> Handle(
        ClaimsPrincipal user,
        IMessageService messageService,
        MainContext context)
    {
        var userId = user.GetUserId();
        if (!userId.HasValue)
            return TypedResults.BadRequest("Invalid user");

        var labubu = await context.Labubus.FirstOrDefaultAsync(l => l.UserId == userId.Value);
        if (labubu == null)
            return TypedResults.NotFound();

        var result = await messageService.MarkMessageAsReadAsync(labubu.Id);

        if (result.IsFailed)
            return TypedResults.NotFound();

        return TypedResults.Ok();
    }
}

