using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using Tenisu.Core.Players;
using Tenisu.Infrastructure;

namespace Tenisu.Application.Tests;

public sealed class TestDb : IAsyncDisposable
{
    private readonly SqliteConnection _connection = new("Data Source=:memory:");

    public TenisuDbContext Context { get; private set; } = default!;

    public static async Task<TestDb> CreateAsync(params Player[] players)
    {
        var db = new TestDb();
        await db._connection.OpenAsync();

        db.Context = new TenisuDbContext(new DbContextOptionsBuilder<TenisuDbContext>().UseSqlite(db._connection).Options);
        await db.Context.Database.EnsureCreatedAsync();

        db.Context.Players.AddRange(players);
        await db.Context.SaveChangesAsync();
        db.Context.ChangeTracker.Clear();

        return db;
    }

    public async ValueTask DisposeAsync()
    {
        await Context.DisposeAsync();
        await _connection.DisposeAsync();
    }
}