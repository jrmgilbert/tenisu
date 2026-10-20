using System.Net;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Tenisu.Infrastructure;

namespace Tenisu.Api.Tests;

public sealed class HealthCheckEndpointTests : IDisposable
{
    private readonly TenisuApiFactory _api = new();

    public void Dispose() => _api.Dispose();

    [Fact]
    public async Task Returns_200_Healthy_when_the_database_answers()
    {
        var response = await _api.CreateClient().GetAsync("/healthcheck", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Healthy", await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task Returns_503_Unhealthy_when_the_database_is_lost()
    {
        var client = _api.CreateClient();

        await using (var scope = _api.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<TenisuDbContext>();
            await db.Database.ExecuteSqlRawAsync("DROP TABLE Players", TestContext.Current.CancellationToken);
        }

        var response = await client.GetAsync("/healthcheck", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
        Assert.Equal("Unhealthy", await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
    }
}