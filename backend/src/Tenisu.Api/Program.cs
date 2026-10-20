using System.Text.Json.Serialization;

using Microsoft.AspNetCore.HttpOverrides;

using Microsoft.OpenTelemetry;

using Scalar.AspNetCore;

using Tenisu.Api;
using Tenisu.Api.Endpoints;
using Tenisu.Application;
using Tenisu.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.UseMicrosoftOpenTelemetry(o => o.AzureMonitor.TracesPerSecond = 5.0);

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddValidation();
builder.Services.AddExceptionHandler<TenisuExceptionHandler>();
builder.Services.AddRateLimitPolicies();

builder.Services.AddResponseCompression(o => o.EnableForHttps = true);

// The Container Apps ingress terminates TLS and is the app's only entry point, so its X-Forwarded-* headers are trusted:
// without them the app believes it is called over http and advertises http:// URLs (e.g. in the OpenAPI document)
builder.Services.Configure<ForwardedHeadersOptions>(o =>
{
    o.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    o.KnownIPNetworks.Clear();
    o.KnownProxies.Clear();
});

builder.Services.ConfigureHttpJsonOptions(o => o.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices();

var app = builder.Build();

await app.Services.InitializeTenisuDataAsync();

app.UseForwardedHeaders();
app.UseResponseCompression();

app.MapOpenApi();
app.MapScalarApiReference();

app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseRateLimiter();

app.MapHealthChecks("/healthcheck");
app.MapPlayerEndpoints();

app.Run();