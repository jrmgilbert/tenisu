using Tenisu.Application.Players;
using Tenisu.Core.Common;
using Tenisu.Core.Tests;

namespace Tenisu.Application.Tests;

public class GetPlayerTests
{
    [Fact]
    public async Task Returns_the_player_for_a_known_id()
    {
        await using var db = await TestDb.CreateAsync(TestPlayers.Fixture());

        var player = await new PlayerQueries(db.Context).GetAsync(102, TestContext.Current.CancellationToken);

        Assert.NotNull(player);
        Assert.Equal("S.WIL", player.ShortName);
        Assert.Equal(Sex.F, player.Sex);
        Assert.Equal("USA", player.Country.Code);
        Assert.Equal([0, 1, 1, 1, 0], player.Data.Last);
    }

    [Fact]
    public async Task Returns_null_for_an_unknown_id()
    {
        await using var db = await TestDb.CreateAsync(TestPlayers.Fixture());

        Assert.Null(await new PlayerQueries(db.Context).GetAsync(999, TestContext.Current.CancellationToken));
    }
}