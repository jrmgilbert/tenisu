using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using Tenisu.Application;
using Tenisu.Application.Common;
using Tenisu.Core.Players;

namespace Tenisu.Infrastructure;

public sealed class TenisuDbContext(DbContextOptions<TenisuDbContext> options) : DbContext(options), ITenisuDbContext
{
    private const int SqliteUniqueConstraintFailed = 2067;

    public DbSet<Player> Players => Set<Player>();

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        try
        {
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
        catch (DbUpdateException e) when (e.InnerException is SqliteException { SqliteExtendedErrorCode: SqliteUniqueConstraintFailed })
        {
            throw new DuplicateKeyException(e);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(TenisuDbContext).Assembly);
}