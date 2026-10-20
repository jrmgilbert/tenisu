using Microsoft.EntityFrameworkCore;

using Tenisu.Application.Common;
using Tenisu.Core.Players;

namespace Tenisu.Application.Players;

public sealed class PlayerCommands(ITenisuDbContext db)
{
    public async Task<PlayerDto> AddAsync(NewPlayer input, CancellationToken ct)
    {
        var player = Player.Create(input.FirstName, input.LastName, input.ShortName, input.Sex,
            Country.Create(input.CountryCode, input.CountryPicture), input.Picture,
            PlayerData.Create(input.Rank, input.Points, input.Weight, input.Height, input.Age, LastResults.From(input.Last)));

        if (await db.Players.AnyAsync(p => p.ShortName == player.ShortName, ct))
            throw ShortNameTaken(player.ShortName);

        db.Players.Add(player);

        try
        {
            await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException)
        {
            // The unique index on ShortName caught a concurrent insert of the same short name
            throw ShortNameTaken(player.ShortName);
        }

        return player.ToDto();
    }

    private static ConflictException ShortNameTaken(string shortName) => new($"Short name '{shortName}' is already taken.");
}