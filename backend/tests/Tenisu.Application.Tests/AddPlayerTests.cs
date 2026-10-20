using Microsoft.EntityFrameworkCore;

using Tenisu.Application.Common;
using Tenisu.Application.Players;
using Tenisu.Core.Common;
using Tenisu.Core.Tests;

namespace Tenisu.Application.Tests;

public class AddPlayerTests
{
    private static NewPlayer Emma(string shortName = "E.ECH", int[]? last = null) =>
        new("Emma", "Echo", shortName, Sex.F, "ITA", "", "", Rank: 5, Points: 1200, Weight: 62000, Height: 172, Age: 24,
            Last: last ?? [1, 0, 1, 1, 0]);

    [Fact]
    public async Task Adds_the_player_with_a_new_id_and_it_can_be_read_back()
    {
        await using var db = await TestDb.CreateAsync(TestPlayers.Fixture());

        var created = await new PlayerCommands(db.Context).AddAsync(Emma(), TestContext.Current.CancellationToken);

        Assert.NotEqual(0, created.Id);
        Assert.DoesNotContain(created.Id, TestPlayers.Fixture().Select(p => p.Id));
        var read = await new PlayerQueries(db.Context).GetAsync(created.Id, TestContext.Current.CancellationToken);
        Assert.Equal("E.ECH", read!.ShortName);
    }

    [Fact]
    public async Task A_taken_short_name_throws_ConflictException()
    {
        await using var db = await TestDb.CreateAsync(TestPlayers.Fixture());

        await Assert.ThrowsAsync<ConflictException>(() =>
            new PlayerCommands(db.Context).AddAsync(Emma("N.DJO"), TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task The_short_name_check_ignores_case_and_spaces()
    {
        await using var db = await TestDb.CreateAsync(TestPlayers.Fixture());

        await Assert.ThrowsAsync<ConflictException>(() =>
            new PlayerCommands(db.Context).AddAsync(Emma(" n.djo "), TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task A_domain_violation_throws_and_saves_nothing()
    {
        await using var db = await TestDb.CreateAsync(TestPlayers.Fixture());

        await Assert.ThrowsAsync<DomainException>(() =>
            new PlayerCommands(db.Context).AddAsync(Emma(last: [2]), TestContext.Current.CancellationToken));
        Assert.Equal(5, await db.Context.Players.CountAsync(TestContext.Current.CancellationToken));
    }
}