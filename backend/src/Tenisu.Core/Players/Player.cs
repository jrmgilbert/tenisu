using Tenisu.Core.Common;

namespace Tenisu.Core.Players;

public sealed class Player
{
    public int Id { get; init; }

    public string FirstName { get; init; } = default!;

    public string LastName { get; init; } = default!;

    public string ShortName { get; init; } = default!;

    public Sex Sex { get; init; }

    public Country Country { get; init; } = default!;

    public string Picture { get; init; } = default!;

    public PlayerData Data { get; init; } = default!;
}