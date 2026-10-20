using Microsoft.EntityFrameworkCore;

using Tenisu.Application.Common;
using Tenisu.Core.Tests;

namespace Tenisu.Application.Tests;

public class SaveChangesTests
{
    [Fact]
    public async Task A_taken_short_name_is_reported_as_a_duplicate_key()
    {
        await using var db = await TestDb.CreateAsync(TestPlayers.Make(1));
        db.Context.Players.Add(TestPlayers.Make(2, shortName: TestPlayers.Make(1).ShortName));

        var failure = await Assert.ThrowsAsync<DuplicateKeyException>(() => db.Context.SaveChangesAsync(TestContext.Current.CancellationToken));

        Assert.IsType<DbUpdateException>(failure.InnerException);
    }

    [Fact]
    public async Task Other_save_failures_keep_their_original_exception()
    {
        await using var db = await TestDb.CreateAsync(TestPlayers.Make(1));
        db.Context.Players.Add(TestPlayers.Make(1, shortName: "Z.ZZZ"));

        await Assert.ThrowsAsync<DbUpdateException>(() => db.Context.SaveChangesAsync(TestContext.Current.CancellationToken));
    }
}