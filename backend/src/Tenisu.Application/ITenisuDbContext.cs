using Microsoft.EntityFrameworkCore;

using Tenisu.Core.Players;

namespace Tenisu.Application;

public interface ITenisuDbContext
{
    DbSet<Player> Players { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}