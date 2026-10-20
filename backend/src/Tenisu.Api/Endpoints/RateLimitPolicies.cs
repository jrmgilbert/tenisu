using System.Globalization;
using System.Threading.RateLimiting;

using Microsoft.AspNetCore.RateLimiting;

namespace Tenisu.Api.Endpoints;

public static class RateLimitPolicies
{
    public const string PlayerWrites = "PlayerWrites";

    public const int PlayerWritesPerMinute = 30;

    public static IServiceCollection AddRateLimitPolicies(this IServiceCollection services) => services.AddRateLimiter(o =>
    {
        o.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
        o.OnRejected = (context, _) =>
        {
            if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                context.HttpContext.Response.Headers.RetryAfter = ((int)Math.Ceiling(retryAfter.TotalSeconds)).ToString(CultureInfo.InvariantCulture);

            return ValueTask.CompletedTask;
        };

        o.AddPolicy(PlayerWrites, http => RateLimitPartition.GetFixedWindowLimiter(
            http.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions { PermitLimit = PlayerWritesPerMinute, Window = TimeSpan.FromMinutes(1) }));
    });
}