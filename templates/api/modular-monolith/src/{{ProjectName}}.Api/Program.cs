using {{ProjectName}}.Modules.Customers.Api;
using {{ProjectName}}.Modules.Orders.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOrdersModule();
builder.Services.AddCustomersModule();

var app = builder.Build();

app.MapOrdersEndpoints();
app.MapCustomersEndpoints();

app.MapGet("/health", () => Results.Ok(new { status = "ok", project = "{{ProjectName}}" }));

app.Run();
