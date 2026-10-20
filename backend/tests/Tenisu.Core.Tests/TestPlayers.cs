using Tenisu.Core.Common;
using Tenisu.Core.Players;

namespace Tenisu.Core.Tests;

public static class TestPlayers
{
    public static Player Make(int id, int rank = 10, int points = 1000, string country = "FRA", int weight = 75000, int height = 180, int[]? last = null) =>
        Create(id, "Test", "Player", $"T.{id:D3}", Sex.M, country, rank, points, weight, height, age: 25, last ?? [], "tbd", "tbd");

    public static Player[] Fixture() =>
    [
        Create(52, "Novak", "Djokovic", "N.DJO", Sex.M, "SRB", rank: 2, points: 2542, weight: 80000, height: 188, age: 31, [1, 1, 1, 1, 1],
            "https://tenisu.latelier.co/resources/Serbie.png", "https://tenisu.latelier.co/resources/Djokovic.png"),
        Create(95, "Venus", "Williams", "V.WIL", Sex.F, "USA", rank: 52, points: 1105, weight: 74000, height: 185, age: 38, [0, 1, 0, 0, 1],
            "https://tenisu.latelier.co/resources/USA.png", "https://tenisu.latelier.co/resources/Venus.webp"),
        Create(65, "Stan", "Wawrinka", "S.WAW", Sex.M, "SUI", rank: 21, points: 1784, weight: 81000, height: 183, age: 33, [1, 1, 1, 0, 1],
            "https://tenisu.latelier.co/resources/Suisse.png", "https://tenisu.latelier.co/resources/Wawrinka.png"),
        Create(102, "Serena", "Williams", "S.WIL", Sex.F, "USA", rank: 10, points: 3521, weight: 72000, height: 175, age: 37, [0, 1, 1, 1, 0],
            "https://tenisu.latelier.co/resources/USA.png", "https://tenisu.latelier.co/resources/Serena.png"),
        Create(17, "Rafael", "Nadal", "R.NAD", Sex.M, "ESP", rank: 1, points: 1982, weight: 85000, height: 185, age: 33, [1, 0, 0, 0, 1],
            "https://tenisu.latelier.co/resources/Espagne.png", "https://tenisu.latelier.co/resources/Nadal.png"),
    ];

    private static Player Create(int id, string firstName, string lastName, string shortName, Sex sex, string countryCode,
        int rank, int points, int weight, int height, int age, int[] last, string countryPicture, string picture) => new()
        {
            Id = id,
            FirstName = firstName,
            LastName = lastName,
            ShortName = shortName,
            Sex = sex,
            Country = new Country(countryCode, countryPicture),
            Picture = picture,
            Data = new PlayerData
            {
                Rank = rank,
                Points = points,
                Weight = weight,
                Height = height,
                Age = age,
                Last = LastResults.From(last)
            }
        };
}