using Tenisu.Core.Common;

namespace Tenisu.Core.Players;

public sealed record LastResults
{
    private readonly bool[] _wins;

    private LastResults(bool[] wins) => _wins = wins;

    public static LastResults From(IEnumerable<int> results)
    {
        var values = results.ToArray();

        if (values.Any(r => r is not (0 or 1)))
            throw new DomainException("Results must be 1 (win) or 0 (loss).");

        return new LastResults(values.Select(r => r == 1).ToArray());
    }

    public static LastResults Parse(string stored) => From(stored.Select(c => c - '0'));

    public override string ToString() => string.Concat(_wins.Select(w => w ? '1' : '0'));
}