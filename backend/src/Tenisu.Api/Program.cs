using System.Text.Json.Serialization;

using Scalar.AspNetCore;

using Tenisu.Api;
using Tenisu.Api.Endpoints;
using Tenisu.Application;
using Tenisu.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddValidation();
builder.Services.AddExceptionHandler<TenisuExceptionHandler>();
builder.Services.ConfigureHttpJsonOptions(o => o.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices();

var app = builder.Build();

await app.Services.InitializeTenisuDataAsync();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseExceptionHandler();
app.UseStatusCodePages();

app.MapHealthChecks("/healthcheck");
app.MapPlayerEndpoints();

app.UseHttpsRedirection();

app.Run();