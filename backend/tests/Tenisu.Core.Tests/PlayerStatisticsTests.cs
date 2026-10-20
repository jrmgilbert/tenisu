using Tenisu.Core.Players;

namespace Tenisu.Core.Tests;

public class PlayerStatisticsTests
{
    [Fact]
    public void Real_players_best_country_is_SRB_with_a_ratio_of_1()
    {
        var stats = PlayerStatistics.Compute(TestPlayers.Fixture());

        Assert.Equal("SRB", stats.BestCountry);
        Assert.Equal(1.0, stats.BestCountryWinRatio);
    }

    [Fact]
    public void Win_ratio_pools_all_matches_of_a_country()
    {
        var stats = PlayerStatistics.Compute(
        [
            TestPlayers.Make(1, country: "ITA", last: [1]),
            TestPlayers.Make(2, country: "ITA", last: [0, 0, 0, 0, 0]),
            TestPlayers.Make(3, country: "USA", last: [1, 1, 0, 0, 0]),
        ]);

        Assert.Equal("USA", stats.BestCountry);
        Assert.Equal(0.4, stats.BestCountryWinRatio);
    }

    [Fact]
    public void A_tie_goes_to_the_alphabetically_first_country()
    {
        var stats = PlayerStatistics.Compute(
        [
            TestPlayers.Make(1, country: "FRA", last: [1, 0]),
            TestPlayers.Make(2, country: "ESP", last: [0, 1]),
        ]);

        Assert.Equal("ESP", stats.BestCountry);
    }

    [Fact]
    public void Countries_without_matches_are_skipped()
    {
        var stats = PlayerStatistics.Compute([TestPlayers.Make(1, country: "FRA", last: [])]);

        Assert.Null(stats.BestCountry);
        Assert.Null(stats.BestCountryWinRatio);
    }

    [Fact]
    public void Bmi_uses_kilograms_and_metres() =>
        Assert.Equal(20.0, PlayerStatistics.Compute([TestPlayers.Make(1, weight: 80000, height: 200)]).AverageBmi);

    [Fact]
    public void Real_players_average_bmi_is_23_36() =>
        Assert.Equal(23.36, PlayerStatistics.Compute(TestPlayers.Fixture()).AverageBmi!.Value, precision: 2);

    [Fact]
    public void Real_players_median_height_is_the_middle_one() =>
        Assert.Equal(185.0, PlayerStatistics.Compute(TestPlayers.Fixture()).MedianHeight);

    [Fact]
    public void Median_height_with_an_even_count_averages_the_middle_two()
    {
        var stats = PlayerStatistics.Compute(
        [
            TestPlayers.Make(1, height: 170),
            TestPlayers.Make(2, height: 190),
            TestPlayers.Make(3, height: 180),
            TestPlayers.Make(4, height: 175),
        ]);

        Assert.Equal(177.5, stats.MedianHeight);
    }

    [Fact]
    public void No_players_gives_all_nulls() =>
        Assert.Equal(new PlayerStatistics(null, null, null, null), PlayerStatistics.Compute([]));
}