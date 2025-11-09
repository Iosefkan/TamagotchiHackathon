using Common.Extensions;
using FluentValidation;
using Labubu.Auth.Services;

namespace Labubu.Auth.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services)
    {
        services.AddJwt(true);

        services.AddTransient<IJwtService, JwtService>();
        services.AddTransient<IRefreshTokenService, RefreshTokenService>();

        services.AddValidatorsFromAssemblyContaining<Program>();

        return services;
    }
}