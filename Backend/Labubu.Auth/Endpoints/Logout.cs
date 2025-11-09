using Common;
using Common.Extensions;
using FluentValidation;
using Labubu.Auth.Extensions;
using Labubu.Auth.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Labubu.Auth.Endpoints;

public class Logout : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost(ApiRoutes.Auth.Logout, Handle)
        .WithSummary("Logout user")
        .WithRequestValidation<Request>();

    private record Request(string refreshToken);

    private class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.refreshToken).NotEmpty().WithMessage("Refresh token is required");
        }
    }

    private static async Task<Results<Ok, UnauthorizedHttpResult>> Handle(
        [FromBody] Request request,
        IRefreshTokenService refreshTokenService,
        CancellationToken cancellationToken)
    {
        var refreshToken = await refreshTokenService.GetValidRefreshTokenAsync(request.refreshToken, cancellationToken);

        if (refreshToken == null)
        {
            return TypedResults.Unauthorized();
        }

        await refreshTokenService.RevokeRefreshTokenAsync(refreshToken, cancellationToken);

        return TypedResults.Ok();
    }
}
