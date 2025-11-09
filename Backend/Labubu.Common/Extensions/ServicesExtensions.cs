using Common.Configuration;
using Common.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Common.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddJwt(this IServiceCollection services, bool withAuth = false)
    {
        services.ConfigureOptions<ConfigureJwtBearerOptions>();

        services.AddTransient<ITokenBlacklistService, TokenBlacklistService>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

        if (withAuth) services.AddAuthorization();

        return services;
    }
    
    public static SwaggerGenOptions AddSecurity(this SwaggerGenOptions options)
    {
        options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter a valid token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = JwtBearerDefaults.AuthenticationScheme
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = JwtBearerDefaults.AuthenticationScheme
                    }
                },
                new string[] { }
            }
        });

        return options;
    }
}