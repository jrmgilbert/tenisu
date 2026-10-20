using Microsoft.EntityFrameworkCore;

using Tenisu.Application;
using Tenisu.Core.Players;

namespace Tenisu.Infrastructure;

public sealed class TenisuDbContext(DbContextOptions<TenisuDbContext> options) : DbContext(options), ITenisuDbContext
{
    public DbSet<Player> Players => Set<Player>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(TenisuDbContext).Assembly);
}