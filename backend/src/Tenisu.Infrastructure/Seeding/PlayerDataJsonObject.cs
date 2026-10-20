namespace Tenisu.Infrastructure.Seeding;

internal sealed record PlayerDataJsonObject(int Rank, int Points, int Weight, int Height, int Age, List<int> Last);