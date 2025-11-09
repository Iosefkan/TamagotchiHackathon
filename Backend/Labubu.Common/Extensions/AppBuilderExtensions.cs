using System.Reflection;
using FluentValidation;
using Microsoft.AspNetCore.Builder;

namespace Common.Extensions;

public static class AppBuilderExtensions
{
    public static WebApplicationBuilder AddValidation(this WebApplicationBuilder builder)
    {
        builder.Services.AddValidatorsFromAssembly(
            builder.Environment.ApplicationName is { } appName
                ? Assembly.Load(appName)
                : Assembly.GetExecutingAssembly(),
            includeInternalTypes: true
        );

        return builder;
    }
}