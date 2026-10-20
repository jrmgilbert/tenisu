using Microsoft.EntityFrameworkCore;

namespace Tenisu.Application.Players;

public sealed class PlayerQueries(ITenisuDbContext db)
{
    public async Task<PlayerDto?> GetAsync(int id, CancellationToken ct) =>
        (await db.Players.AsNoTracking().SingleOrDefaultAsync(p => p.Id == id, ct))?.ToDto();
}