using System.Text.Json;

namespace Tenisu.Api.Tests;

public class PlayerStatisticsEndpointTests(TenisuApiFactory api, EmptyTenisuApiFactory empty)
    : IClassFixture<TenisuApiFactory>, IClassFixture<EmptyTenisuApiFactory>
{
    [Fact]
    public async Task Returns_200_with_the_statistics()
    {
        var json = await ApiAssert.OkJsonAsync(
            await api.CreateClient().GetAsync("/api/players/statistics", TestContext.Current.CancellationToken));

        Assert.Equal("SRB", json.GetProperty("bestCountry").GetString());
        Assert.Equal(1.0, json.GetProperty("bestCountryWinRatio").GetDouble());
        Assert.Equal(23.36, json.GetProperty("averageBmi").GetDouble());
        Assert.Equal(185.0, json.GetProperty("medianHeight").GetDouble());
    }

    [Fact]
    public async Task Returns_200_with_nulls_when_there_are_no_players()
    {
        var json = await ApiAssert.OkJsonAsync(
            await empty.CreateClient().GetAsync("/api/players/statistics", TestContext.Current.CancellationToken));

        Assert.Equal(JsonValueKind.Null, json.GetProperty("bestCountry").ValueKind);
        Assert.Equal(JsonValueKind.Null, json.GetProperty("bestCountryWinRatio").ValueKind);
        Assert.Equal(JsonValueKind.Null, json.GetProperty("averageBmi").ValueKind);
        Assert.Equal(JsonValueKind.Null, json.GetProperty("medianHeight").ValueKind);
    }
}