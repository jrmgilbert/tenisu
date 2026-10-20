using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace Tenisu.Api.Tests;

public class PlayerCapacityEndpointTests(FullTenisuApiFactory full) : IClassFixture<FullTenisuApiFactory>
{
    [Fact]
    public async Task Adding_to_a_full_player_list_returns_409_problem()
    {
        var response = await full.CreateClient().PostAsJsonAsync("/api/players", ValidPlayer("E.ECH"), TestContext.Current.CancellationToken);

        ApiAssert.IsProblem(response, HttpStatusCode.Conflict);
        var problem = await response.Content.ReadFromJsonAsync<JsonElement>(TestContext.Current.CancellationToken);
        Assert.Contains("100", problem.GetProperty("detail").GetString());
    }

    internal static object ValidPlayer(string shortName) => new
    {
        firstName = "Emma",
        lastName = "Echo",
        shortName,
        sex = "F",
        country = new { code = "ITA" },
        data = new { rank = 5, points = 1200, weight = 62000, height = 172, age = 24, last = new[] { 1, 0 } }
    };
}