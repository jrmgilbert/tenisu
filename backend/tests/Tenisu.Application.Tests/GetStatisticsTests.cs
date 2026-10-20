using Tenisu.Application.Players;
using Tenisu.Core.Tests;

namespace Tenisu.Application.Tests;

public class GetStatisticsTests
{
    [Fact]
    public async Task Returns_the_real_players_statistics_rounded()
    {
        await using var db = await TestDb.CreateAsync(TestPlayers.Fixture());

        var stats = await new PlayerQueries(db.Context).GetStatisticsAsync(TestContext.Current.CancellationToken);

        Assert.Equal(new PlayerStatisticsDto("SRB", 1.0, 23.36, 185.0), stats);
    }

    [Fact]
    public async Task Returns_nulls_when_there_are_no_players()
    {
        await using var db = await TestDb.CreateAsync();

        var stats = await new PlayerQueries(db.Context).GetStatisticsAsync(TestContext.Current.CancellationToken);

        Assert.Equal(new PlayerStatisticsDto(null, null, null, null), stats);
    }
}