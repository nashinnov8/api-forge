using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace {{ProjectName}}.Modules.Orders.Api;

public static class OrdersModule
{
    public static IServiceCollection AddOrdersModule(this IServiceCollection services)
    {
        return services;
    }

    public static IEndpointRouteBuilder MapOrdersEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/orders", () => Results.Ok(new[] { new { id = Guid.NewGuid(), status = "pending" } }));
        return endpoints;
    }
}
