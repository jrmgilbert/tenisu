using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Tenisu.Application;
using Tenisu.Infrastructure.Health;
using Tenisu.Infrastructure.Persistence;
using Tenisu.Infrastructure.Seeding;

namespace Tenisu.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddSingleton(sp => TenisuDataSettings.From(sp.GetRequiredService<IConfiguration>()));

        services.AddSingleton(sp => new SqliteConnectionManager(sp.GetRequiredService<TenisuDataSettings>().ConnectionString));

        services.AddDbContext<TenisuDbContext>((sp, options) =>
        {
            var settings = sp.GetRequiredService<TenisuDataSettings>();

            options.UseSqlite(settings.ConnectionString)
                   .UseAsyncSeeding((ctx, _, ct) => HeadToHeadDataSeeder.SeedAsync(ctx, settings.DataFilePath, ct));
        });

        services.AddScoped<ITenisuDbContext>(sp => sp.GetRequiredService<TenisuDbContext>());

        services.AddHealthChecks().AddCheck<TenisuDatabaseHealthCheck>("sqlite");

        return services;
    }

    public static async Task InitializeTenisuDataAsync(this IServiceProvider services, CancellationToken ct = default)
    {
        services.GetRequiredService<SqliteConnectionManager>();

        await using var scope = services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<TenisuDbContext>();

        await db.Database.EnsureCreatedAsync(ct);
    }
}