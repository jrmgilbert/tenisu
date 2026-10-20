using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Tenisu.Infrastructure.Health;

internal sealed class TenisuDatabaseHealthCheck(TenisuDbContext db) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var players = await db.Players.CountAsync(cancellationToken);

        return HealthCheckResult.Healthy($"{players} players in the SQLite database.");
    }
}