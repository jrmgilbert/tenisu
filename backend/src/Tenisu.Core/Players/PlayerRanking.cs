namespace Tenisu.Core.Players;

public sealed class PlayerRanking : IComparer<Player>
{
    public static PlayerRanking BestFirst { get; } = new();

    private PlayerRanking() { }

    public int Compare(Player? x, Player? y)
    {
        if (ReferenceEquals(x, y))
            return 0;
        if (x is null)
            return 1;
        if (y is null)
            return -1;

        var byRank = x.Data.Rank.CompareTo(y.Data.Rank);
        if (byRank != 0)
            return byRank;

        var byPoints = y.Data.Points.CompareTo(x.Data.Points);
        return byPoints != 0 ? byPoints : x.Id.CompareTo(y.Id);
    }
}