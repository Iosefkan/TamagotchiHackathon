using Common;
using Common.Extensions;
using FluentValidation;
using Labubu.Auth.Data;
using Labubu.Auth.Data.Models;
using Labubu.Auth.Extensions;
using Labubu.Auth.Services;
using MassTransit;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Contracts;

namespace Labubu.Auth.Endpoints;

public class Signup : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost(ApiRoutes.Auth.Signup, Handle)
        .WithSummary("Register a new user")
        .WithRequestValidation<Request>();

    private record Request(string Username, string Email, string Password);

    private record Response(string accessToken, string refreshToken);

    private class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required")
                .Length(3, 50).WithMessage("Username must be between 3 and 50 characters");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters");
        }
    }

    private static async Task<Results<Ok<Response>, BadRequest<string>>> Handle(
        [FromBody] Request request,
        AuthDbContext database,
        IJwtService jwtService,
        IRefreshTokenService refreshTokenService,
        IPublishEndpoint publishEndpoint,
        ILogger<Signup> logger,
        CancellationToken cancellationToken)
    {
        // Check if username already exists
        if (await database.Users.AnyAsync(x => x.Username == request.Username, cancellationToken))
        {
            return TypedResults.BadRequest("Username is already taken");
        }

        // Check if email already exists
        if (await database.Users.AnyAsync(x => x.Email == request.Email, cancellationToken))
        {
            return TypedResults.BadRequest("Email is already registered");
        }

        // Hash password
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        // Create user
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            Email = request.Email,
            PasswordHash = passwordHash,
            CreatedAt = DateTime.UtcNow
        };

        await database.Users.AddAsync(user, cancellationToken);
        await database.SaveChangesAsync(cancellationToken);

        // Send message to other services via RabbitMQ
        try
        {
            await publishEndpoint.Publish(new AddUserRequest
            {
                UserId = user.Id,
                Username = user.Username,
                Email = user.Email
            }, cancellationToken);
            
            logger.LogInformation("User {Username} registered and message sent to RabbitMQ", user.Username);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send user registration message to RabbitMQ for user {Username}", user.Username);
            // Continue with registration even if RabbitMQ fails
        }

        // Generate tokens
        var accessToken = jwtService.GenerateAccessToken(user);
        var refreshToken = await refreshTokenService.CreateRefreshTokenAsync(user.Id, cancellationToken);

        var response = new Response(accessToken, refreshToken.Token);
        return TypedResults.Ok(response);
    }
}

