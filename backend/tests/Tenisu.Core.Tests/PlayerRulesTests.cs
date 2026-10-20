using Tenisu.Core.Common;
using Tenisu.Core.Players;

namespace Tenisu.Core.Tests;

public class PlayerRulesTests
{
    private static Country Italy => Country.Create("ITA", "");

    private static PlayerData ValidData => PlayerData.Create(5, 1200, 62000, 172, 24, LastResults.From([1, 0]));

    [Fact]
    public void Last_results_accept_only_0_and_1() =>
        Assert.Throws<DomainException>(() => LastResults.From([1, 2]));

    [Fact]
    public void Last_results_hold_at_most_MaxResults_results() =>
        Assert.Throws<DomainException>(() => LastResults.From(Enumerable.Repeat(1, LastResults.MaxResults + 1)));

    [Theory]
    [InlineData(0, 0, 62000, 172, 24)]
    [InlineData(5, -1, 62000, 172, 24)]
    [InlineData(5, 0, 0, 172, 24)]
    [InlineData(5, 0, 62000, 0, 24)]
    [InlineData(5, 0, 62000, 172, 0)]
    public void Data_rejects_out_of_range_values(int rank, int points, int weight, int height, int age) =>
        Assert.Throws<DomainException>(() => PlayerData.Create(rank, points, weight, height, age, LastResults.From([])));

    [Theory]
    [InlineData("srb")]
    [InlineData("FRAN")]
    [InlineData("")]
    public void Country_code_is_3_capital_letters(string code) =>
        Assert.Throws<DomainException>(() => Country.Create(code, ""));

    [Theory]
    [InlineData(" ", "Echo")]
    [InlineData("Emma", "")]
    public void Player_requires_first_and_last_name(string firstName, string lastName) =>
        Assert.Throws<DomainException>(() => Player.Create(firstName, lastName, "E.ECH", Sex.F, Italy, "", ValidData));

    [Theory]
    [InlineData("NECH")]
    [InlineData("E.EC")]
    [InlineData("")]
    public void Player_requires_a_short_name_like_N_DJO(string shortName) =>
        Assert.Throws<DomainException>(() => Player.Create("Emma", "Echo", shortName, Sex.F, Italy, "", ValidData));

    [Fact]
    public void Player_rejects_an_undefined_sex() =>
        Assert.Throws<DomainException>(() => Player.Create("Emma", "Echo", "E.ECH", (Sex)7, Italy, "", ValidData));

    [Fact]
    public void Names_are_trimmed_and_the_short_name_is_upper_case()
    {
        var player = Player.Create(" Emma ", " Echo ", " e.ech ", Sex.F, Italy, "", ValidData);

        Assert.Equal("Emma", player.FirstName);
        Assert.Equal("Echo", player.LastName);
        Assert.Equal("E.ECH", player.ShortName);
    }
}