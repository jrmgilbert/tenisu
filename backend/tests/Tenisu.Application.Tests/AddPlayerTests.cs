using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Testing;

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

        var created = await new PlayerCommands(db.Context, NullLogger<PlayerCommands>.Instance).AddAsync(Emma(), TestContext.Current.CancellationToken);

        Assert.NotEqual(0, created.Id);
        Assert.DoesNotContain(created.Id, TestPlayers.Fixture().Select(p => p.Id));
        var read = await new PlayerQueries(db.Context).GetAsync(created.Id, TestContext.Current.CancellationToken);
        Assert.Equal("E.ECH", read!.ShortName);
    }

    [Fact]
    public async Task Adding_a_player_logs_it_at_information()
    {
        await using var db = await TestDb.CreateAsync(TestPlayers.Fixture());
        var logger = new FakeLogger<PlayerCommands>();

        var created = await new PlayerCommands(db.Context, logger).AddAsync(Emma(), TestContext.Current.CancellationToken);

        var record = logger.Collector.LatestRecord;
        Assert.Equal(LogLevel.Information, record.Level);
        Assert.Equal("PlayerAdded", record.Id.Name);
        Assert.Equal($"Player E.ECH added with id {created.Id}", record.Message);
    }

    [Fact]
    public async Task A_taken_short_name_throws_ConflictException()
    {
        await using var db = await TestDb.CreateAsync(TestPlayers.Fixture());

        await Assert.ThrowsAsync<ConflictException>(() =>
            new PlayerCommands(db.Context, NullLogger<PlayerCommands>.Instance).AddAsync(Emma("N.DJO"), TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task The_short_name_check_ignores_case_and_spaces()
    {
        await using var db = await TestDb.CreateAsync(TestPlayers.Fixture());

        await Assert.ThrowsAsync<ConflictException>(() =>
            new PlayerCommands(db.Context, NullLogger<PlayerCommands>.Instance).AddAsync(Emma(" n.djo "), TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task The_last_free_slot_can_be_taken()
    {
        await using var db = await TestDb.CreateAsync([.. Enumerable.Range(1, PlayerCommands.MaxPlayers - 1).Select(id => TestPlayers.Make(id))]);

        await new PlayerCommands(db.Context, NullLogger<PlayerCommands>.Instance).AddAsync(Emma(), TestContext.Current.CancellationToken);

        Assert.Equal(PlayerCommands.MaxPlayers, await db.Context.Players.CountAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task A_full_player_list_throws_ConflictException_and_saves_nothing()
    {
        await using var db = await TestDb.CreateAsync([.. Enumerable.Range(1, PlayerCommands.MaxPlayers).Select(id => TestPlayers.Make(id))]);

        await Assert.ThrowsAsync<ConflictException>(() =>
            new PlayerCommands(db.Context, NullLogger<PlayerCommands>.Instance).AddAsync(Emma(), TestContext.Current.CancellationToken));
        Assert.Equal(PlayerCommands.MaxPlayers, await db.Context.Players.CountAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task A_domain_violation_throws_and_saves_nothing()
    {
        await using var db = await TestDb.CreateAsync(TestPlayers.Fixture());

        await Assert.ThrowsAsync<DomainException>(() =>
            new PlayerCommands(db.Context, NullLogger<PlayerCommands>.Instance).AddAsync(Emma(last: [2]), TestContext.Current.CancellationToken));
        Assert.Equal(5, await db.Context.Players.CountAsync(TestContext.Current.CancellationToken));
    }
}