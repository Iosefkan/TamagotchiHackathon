using Common;
using Common.Extensions;
using FluentValidation;
using Labubu.Auth.Data;
using Labubu.Auth.Extensions;
using Labubu.Auth.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Labubu.Auth.Endpoints;

public class Signin : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost(ApiRoutes.Auth.Signin, Handle)
        .WithSummary("Login user")
        .WithRequestValidation<Request>();

    private record Request(string Username, string Password);

    private record Response(string accessToken, string refreshToken);

    private class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Username).NotEmpty().WithMessage("Username is required");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required");
        }
    }

    private static async Task<Results<Ok<Response>, UnauthorizedHttpResult>> Handle(
        [FromBody] Request request,
        AuthDbContext database,
        IJwtService jwtService,
        IRefreshTokenService refreshTokenService,
        CancellationToken cancellationToken)
    {
        // Find user by username or email
        var user = await database.Users
            .FirstOrDefaultAsync(
                x => x.Username == request.Username || x.Email == request.Username, 
                cancellationToken);

        if (user == null)
        {
            return TypedResults.Unauthorized();
        }

        // Verify password
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return TypedResults.Unauthorized();
        }

        // Generate tokens
        var accessToken = jwtService.GenerateAccessToken(user);
        var refreshToken = await refreshTokenService.CreateRefreshTokenAsync(user.Id, cancellationToken);

        var response = new Response(accessToken, refreshToken.Token);
        return TypedResults.Ok(response);
    }
}

