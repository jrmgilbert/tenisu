namespace Tenisu.Core.Players;

public sealed record PlayerData
{
    public int Rank { get; init; }

    public int Points { get; init; }

    public int Weight { get; init; }

    public int Height { get; init; }

    public int Age { get; init; }

    public LastResults Last { get; init; } = default!;

    public double Bmi => Weight / 1000.0 / Math.Pow(Height / 100.0, 2);
}