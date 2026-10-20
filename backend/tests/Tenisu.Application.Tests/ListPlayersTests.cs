using Tenisu.Application.Players;
using Tenisu.Core.Tests;

namespace Tenisu.Application.Tests;

public class ListPlayersTests
{
    [Fact]
    public async Task Best_first_returns_players_by_rank_then_points_then_id()
    {
        await using var db = await TestDb.CreateAsync(TestPlayers.Fixture());

        var players = await new PlayerQueries(db.Context).ListAsync(PlayerSort.BestFirst, TestContext.Current.CancellationToken);

        Assert.Equal([17, 52, 102, 65, 95], players.Select(p => p.Id));
    }

    [Fact]
    public async Task Returns_an_empty_list_when_there_are_no_players()
    {
        await using var db = await TestDb.CreateAsync();

        Assert.Empty(await new PlayerQueries(db.Context).ListAsync(PlayerSort.BestFirst, TestContext.Current.CancellationToken));
    }
}