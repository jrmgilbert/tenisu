using Microsoft.EntityFrameworkCore;

using Tenisu.Core.Players;

namespace Tenisu.Application.Tests;

public sealed class FailingSaveDb(ITenisuDbContext inner, Exception failure) : ITenisuDbContext
{
    public DbSet<Player> Players => inner.Players;

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => Task.FromException<int>(failure);
}