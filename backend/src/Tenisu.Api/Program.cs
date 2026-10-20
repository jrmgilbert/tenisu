using Tenisu.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddInfrastructureServices();

var app = builder.Build();

await app.Services.InitializeTenisuDataAsync();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapHealthChecks("/healthcheck");

app.UseHttpsRedirection();

app.Run();