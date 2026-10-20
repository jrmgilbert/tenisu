namespace Tenisu.Core.Players;

public static class PlayerOrderings
{
    public static IOrderedQueryable<Player> OrderByBestFirst(this IQueryable<Player> players) =>
        players.OrderBy(p => p.Data.Rank).ThenByDescending(p => p.Data.Points).ThenBy(p => p.Id);
}