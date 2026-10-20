namespace Tenisu.Application.Players;

public sealed record PlayerDataDto(int Rank, int Points, int Weight, int Height, int Age, IReadOnlyList<int> Last);