using Common.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Common.Extensions;

public static class EndpointBuilderExtensions
{
    public static RouteHandlerBuilder WithRequestValidation<TRequest>(this RouteHandlerBuilder builder)
        where TRequest : class
    {
        return builder
            .AddEndpointFilter<RequestValidationFilter<TRequest>>()
            .ProducesValidationProblem();
    }
}