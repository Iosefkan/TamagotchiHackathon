using Common;
using Common.Extensions;
using Labubu.Main.Extensions;
using Labubu.Main.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Labubu.Main.Endpoints;

public class GetUserInfo : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapGet(ApiRoutes.Users.Info, Handle)
        .RequireAuthorization()
        .WithSummary("Get user information");

    private record BankResponse(int bankId, string bankName, decimal balance);
    private record Response(string login, string email, List<BankResponse> banks);

    private static async Task<Results<Ok<Response>, NotFound, BadRequest<string>>> Handle(
        ClaimsPrincipal user,
        IUserService userService)
    {
        var userId = user.GetUserId();
        if (!userId.HasValue)
            return TypedResults.BadRequest("Invalid user");

        var result = await userService.GetUserInfoAsync(userId.Value);

        if (result.IsFailed)
            return TypedResults.NotFound();

        var dto = result.Value;
        var banks = dto.Banks.Select(b => new BankResponse(
            b.BankId,
            b.BankName,
            b.Balance
        )).ToList();

        var response = new Response(
            dto.Login,
            dto.Email,
            banks
        );

        return TypedResults.Ok(response);
    }
}

