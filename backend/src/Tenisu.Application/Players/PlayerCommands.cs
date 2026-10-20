using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Tenisu.Application.Common;
using Tenisu.Core.Players;

namespace Tenisu.Application.Players;

public sealed partial class PlayerCommands(ITenisuDbContext db, ILogger<PlayerCommands> logger)
{
    public const int MaxPlayers = 100;

    public async Task<PlayerDto> AddAsync(NewPlayer input, CancellationToken ct)
    {
        var player = Player.Create(input.FirstName, input.LastName, input.ShortName, input.Sex,
            Country.Create(input.CountryCode, input.CountryPicture), input.Picture,
            PlayerData.Create(input.Rank, input.Points, input.Weight, input.Height, input.Age, LastResults.From(input.Last)));

        if (await db.Players.CountAsync(ct) >= MaxPlayers)
            throw new ConflictException($"The player list is full: {MaxPlayers} players at most.");

        if (await db.Players.AnyAsync(p => p.ShortName == player.ShortName, ct))
            throw ShortNameTaken(player.ShortName);

        db.Players.Add(player);

        try
        {
            await db.SaveChangesAsync(ct);
        }
        catch (DuplicateKeyException)
        {
            // The unique index on ShortName caught a concurrent insert of the same short name
            throw ShortNameTaken(player.ShortName);
        }

        LogPlayerAdded(logger, player.ShortName, player.Id);

        return player.ToDto();
    }

    private static ConflictException ShortNameTaken(string shortName) => new($"Short name '{shortName}' is already taken.");

    [LoggerMessage(EventId = 1001, EventName = "PlayerAdded", Level = LogLevel.Information, Message = "Player {ShortName} added with id {PlayerId}")]
    private static partial void LogPlayerAdded(ILogger logger, string shortName, int playerId);
}