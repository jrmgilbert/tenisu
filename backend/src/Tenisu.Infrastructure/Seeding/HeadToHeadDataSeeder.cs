using System.Text.Json;

using Microsoft.EntityFrameworkCore;

using Tenisu.Core.Common;
using Tenisu.Core.Players;

namespace Tenisu.Infrastructure.Seeding;

internal static class HeadToHeadDataSeeder
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public static async Task SeedAsync(DbContext context, string filePath, CancellationToken ct)
    {
        var db = (TenisuDbContext)context;

        if (await db.Players.AnyAsync(ct))
            return;

        db.Players.AddRange(Load(filePath));

        await db.SaveChangesAsync(ct);
    }

    private static List<Player> Load(string filePath)
    {
        using var stream = File.OpenRead(filePath);

        var seed = JsonSerializer.Deserialize<HeadToHeadJsonObject>(stream, JsonOptions)
                   ?? throw new InvalidOperationException($"'{filePath}' is empty.");

        return seed.Players.Select(p => Player.Create(
            p.Firstname,
            p.Lastname,
            p.Shortname,
            ParseSex(p.Sex),
            Country.Create(p.Country.Code, p.Country.Picture),
            p.Picture,
            PlayerData.Create(p.Data.Rank, p.Data.Points, p.Data.Weight, p.Data.Height, p.Data.Age, LastResults.From(p.Data.Last)),
            p.Id)).ToList();
    }

    private static Sex ParseSex(string value) => value switch
    {
        "M" => Sex.M,
        "F" => Sex.F,
        _ => throw new InvalidOperationException($"Unknown value '{value}' for \"sex\" in the data file.")
    };
}