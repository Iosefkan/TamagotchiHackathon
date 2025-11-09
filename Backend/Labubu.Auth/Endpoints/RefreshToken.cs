using Common;
using Common.Extensions;
using FluentValidation;
using Labubu.Auth.Extensions;
using Labubu.Auth.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Labubu.Auth.Endpoints;

public class RefreshToken : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost(ApiRoutes.Auth.Refresh, Handle)
        .WithSummary("Refresh access token")
        .WithRequestValidation<Request>();

    private record Request(string accessToken, string refreshToken);

    private record Response(string accessToken, string refreshToken);

    private class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.refreshToken).NotEmpty().WithMessage("Refresh token is required");
            RuleFor(x => x.accessToken).NotEmpty().WithMessage("Access token is required");
        }
    }

    private static async Task<Results<Ok<Response>, UnauthorizedHttpResult>> Handle(
        [FromBody] Request request,
        IJwtService jwtService,
        IRefreshTokenService refreshTokenService,
        CancellationToken cancellationToken)
    {
        // Validate access token and get principal
        var principal = jwtService.GetPrincipalFromToken(request.accessToken);
        if (principal == null)
        {
            return TypedResults.Unauthorized();
        }

        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
        {
            return TypedResults.Unauthorized();
        }

        // Get valid refresh token
        var storedRefreshToken = await refreshTokenService.GetValidRefreshTokenAsync(request.refreshToken, cancellationToken);
        if (storedRefreshToken == null || storedRefreshToken.UserId != userId)
        {
            return TypedResults.Unauthorized();
        }

        // Revoke old refresh token
        await refreshTokenService.RevokeRefreshTokenAsync(storedRefreshToken, cancellationToken);

        // Generate new tokens
        var user = storedRefreshToken.User;
        var newAccessToken = jwtService.GenerateAccessToken(user);
        var newRefreshToken = await refreshTokenService.CreateRefreshTokenAsync(user.Id, cancellationToken);

        var response = new Response(newAccessToken, newRefreshToken.Token);
        return TypedResults.Ok(response);
    }
}

