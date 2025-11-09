using Microsoft.AspNetCore.Routing;

namespace Common;

public interface IEndpoint
{
    static abstract void Map(IEndpointRouteBuilder app);
}