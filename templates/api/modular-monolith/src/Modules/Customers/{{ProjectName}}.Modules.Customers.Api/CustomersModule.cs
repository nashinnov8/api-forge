using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace {{ProjectName}}.Modules.Customers.Api;

public static class CustomersModule
{
    public static IServiceCollection AddCustomersModule(this IServiceCollection services)
    {
        return services;
    }

    public static IEndpointRouteBuilder MapCustomersEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/customers", () => Results.Ok(new[] { new { id = Guid.NewGuid(), name = "Acme Corp" } }));
        return endpoints;
    }
}
