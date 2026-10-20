using Tenisu.Core.Common;

namespace Tenisu.Core.Players;

public sealed record PlayerData
{
    public int Rank { get; private init; }

    public int Points { get; private init; }

    public int Weight { get; private init; }

    public int Height { get; private init; }

    public int Age { get; private init; }

    public LastResults Last { get; private init; } = default!;

    public double Bmi => Weight / 1000.0 / Math.Pow(Height / 100.0, 2);

    private PlayerData() { }

    public static PlayerData Create(int rank, int points, int weight, int height, int age, LastResults last)
    {
        if (rank < 1)
            throw new DomainException("Rank starts at 1.");
        if (points < 0)
            throw new DomainException("Points cannot be negative.");
        if (weight <= 0 || height <= 0 || age <= 0)
            throw new DomainException("Weight, height and age must be positive.");

        return new()
        {
            Rank = rank,
            Points = points,
            Weight = weight,
            Height = height,
            Age = age,
            Last = last
        };
    }
}