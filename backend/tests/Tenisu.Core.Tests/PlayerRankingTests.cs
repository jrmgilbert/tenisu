using Tenisu.Core.Players;

namespace Tenisu.Core.Tests;

public class PlayerRankingTests
{
    private static int Compare(Player x, Player y) => PlayerRanking.BestFirst.Compare(x, y);

    [Fact]
    public void Lower_rank_comes_first() =>
        Assert.True(Compare(TestPlayers.Make(1, rank: 1), TestPlayers.Make(2, rank: 2)) < 0);

    [Fact]
    public void Same_rank_more_points_comes_first() =>
        Assert.True(Compare(TestPlayers.Make(1, rank: 1, points: 100), TestPlayers.Make(2, rank: 1, points: 900)) > 0);

    [Fact]
    public void Same_rank_and_points_lower_id_comes_first() =>
        Assert.True(Compare(TestPlayers.Make(7, rank: 1, points: 100), TestPlayers.Make(3, rank: 1, points: 100)) > 0);

    [Fact]
    public void A_player_ranks_equal_to_itself()
    {
        var player = TestPlayers.Make(1);

        Assert.Equal(0, Compare(player, player));
    }

    [Fact]
    public void Real_players_sort_to_17_52_102_65_95() =>
        Assert.Equal([17, 52, 102, 65, 95], TestPlayers.Fixture().Order(PlayerRanking.BestFirst).Select(p => p.Id));
}