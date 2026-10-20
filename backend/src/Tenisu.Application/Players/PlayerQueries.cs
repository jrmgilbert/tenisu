using Microsoft.EntityFrameworkCore;

using Tenisu.Core.Players;

namespace Tenisu.Application.Players;

public sealed class PlayerQueries(ITenisuDbContext db)
{
    public async Task<IReadOnlyList<PlayerDto>> ListAsync(PlayerSort sort, CancellationToken ct)
    {
        var players = db.Players.AsNoTracking();

        var sorted = sort switch
        {
            PlayerSort.BestFirst => players.OrderByBestFirst(),
            _ => throw new ArgumentOutOfRangeException(nameof(sort), sort, null)
        };

        return [.. (await sorted.ToListAsync(ct)).Select(p => p.ToDto())];
    }

    public async Task<PlayerStatisticsDto> GetStatisticsAsync(CancellationToken ct) =>
        PlayerStatistics.Compute(await db.Players.AsNoTracking().ToListAsync(ct)).ToDto();

    public async Task<PlayerDto?> GetAsync(int id, CancellationToken ct) =>
        (await db.Players.AsNoTracking().SingleOrDefaultAsync(p => p.Id == id, ct))?.ToDto();
}