using Tenisu.Core.Players;

namespace Tenisu.Core.Tests;

public class PlayerOrderingsTests
{
    [Fact]
    public void Best_first_orders_real_players_to_17_52_102_65_95() =>
        Assert.Equal([17, 52, 102, 65, 95], TestPlayers.Fixture().AsQueryable().OrderByBestFirst().Select(p => p.Id));

    [Fact]
    public void Best_first_matches_the_in_memory_ranking()
    {
        Player[] players =
        [
            TestPlayers.Make(7, rank: 1, points: 100),
            TestPlayers.Make(5, rank: 2, points: 900),
            TestPlayers.Make(3, rank: 1, points: 100),
            TestPlayers.Make(2, rank: 1, points: 900),
        ];

        Assert.Equal(
            players.Order(PlayerRanking.BestFirst).Select(p => p.Id),
            players.AsQueryable().OrderByBestFirst().Select(p => p.Id));
    }
}