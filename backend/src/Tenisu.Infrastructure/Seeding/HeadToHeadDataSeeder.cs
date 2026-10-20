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

        return seed.Players.Select(p => new Player
        {
            Id = p.Id,
            FirstName = p.Firstname,
            LastName = p.Lastname,
            ShortName = p.Shortname,
            Sex = ParseSex(p.Sex),
            Country = new Country(p.Country.Code, p.Country.Picture),
            Picture = p.Picture,
            Data = new PlayerData
            {
                Rank = p.Data.Rank,
                Points = p.Data.Points,
                Weight = p.Data.Weight,
                Height = p.Data.Height,
                Age = p.Data.Age,
                Last = LastResults.From(p.Data.Last)
            }
        }).ToList();
    }

    private static Sex ParseSex(string value) => value switch
    {
        "M" => Sex.M,
        "F" => Sex.F,
        _ => throw new InvalidOperationException($"Unknown value '{value}' for \"sex\" in the data file.")
    };
}