namespace Tenisu.Core.Players;

public sealed record PlayerStatistics(string? BestCountry, double? BestCountryWinRatio, double? AverageBmi, double? MedianHeight)
{
    public static PlayerStatistics Compute(IReadOnlyCollection<Player> players)
    {
        if (players.Count == 0)
            return new(null, null, null, null);

        var bestCountry = players
            .GroupBy(p => p.Country.Code)
            .Select(g => new CountryForm(g.Key, g.Sum(p => p.Data.Last.Wins), g.Sum(p => p.Data.Last.Played)))
            .Where(c => c.Played > 0)
            .OrderByDescending(c => c.WinRatio)
            .ThenBy(c => c.Code, StringComparer.Ordinal)
            .FirstOrDefault();

        var heights = players.Select(p => p.Data.Height).Order().ToArray();
        var middle = heights.Length / 2;
        var medianHeight = heights.Length % 2 == 0 ? (heights[middle - 1] + heights[middle]) / 2.0 : heights[middle];

        return new(bestCountry?.Code, bestCountry?.WinRatio, players.Average(p => p.Data.Bmi), medianHeight);
    }
}