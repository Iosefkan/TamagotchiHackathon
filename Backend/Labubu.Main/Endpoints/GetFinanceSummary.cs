using System.Linq;
using Common;
using Common.Extensions;
using Labubu.Main.Extensions;
using Labubu.Main.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Labubu.Main.Endpoints;

public class GetFinanceSummary : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapGet(ApiRoutes.Finance.Summary, Handle)
        .RequireAuthorization()
        .WithSummary("Get finance dashboard data");

    private record UserResponse(Guid Id, string Name, string Email, string? Picture);
    private record BankResponse(string Id, string Name);
    private record TransactionResponse(
        string Uuid,
        string BankId,
        string Name,
        string Category,
        string Type,
        decimal Amount,
        string Status,
        string Date);

    private record Response(
        UserResponse User,
        decimal Balance,
        List<BankResponse> Banks,
        List<TransactionResponse> Transactions);

    private static async Task<Results<Ok<Response>, BadRequest<string>>> Handle(
        ClaimsPrincipal user,
        IFinanceService financeService)
    {
        var userId = user.GetUserId();

        if (!userId.HasValue)
        {
            return TypedResults.BadRequest("Invalid user");
        }

        var summaryResult = await financeService.GetSummaryAsync(userId.Value);

        if (summaryResult.IsFailed)
        {
            var errorMessage = summaryResult.Errors.FirstOrDefault()?.Message ?? "Unable to build summary";
            return TypedResults.BadRequest(errorMessage);
        }

        var summary = summaryResult.Value;

        var response = new Response(
            new UserResponse(
                summary.User.Id,
                summary.User.Name,
                summary.User.Email,
                summary.User.Picture),
            summary.Balance,
            summary.Banks
                .Select(b => new BankResponse(b.Id, b.Name))
                .ToList(),
            summary.Transactions
                .Select(t => new TransactionResponse(
                    t.Uuid,
                    t.BankId,
                    t.Name,
                    t.Category,
                    t.Type,
                    t.Amount,
                    t.Status,
                    t.Date.ToString("O")))
                .ToList());

        return TypedResults.Ok(response);
    }
}

