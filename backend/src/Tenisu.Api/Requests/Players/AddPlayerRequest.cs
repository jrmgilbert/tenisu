using System.ComponentModel.DataAnnotations;

using Tenisu.Application.Players;
using Tenisu.Core.Common;
using Tenisu.Core.Players;

namespace Tenisu.Api.Requests.Players;

public sealed record AddPlayerRequest
{
    [Required, StringLength(50, MinimumLength = 1)]
    public string? FirstName { get; init; }

    [Required, StringLength(50, MinimumLength = 1)]
    public string? LastName { get; init; }

    [Required, RegularExpression(Player.ShortNamePattern, ErrorMessage = "Use an initial, a dot and 3 capital letters, e.g. N.DJO.")]
    public string? ShortName { get; init; }

    [Required, EnumDataType(typeof(Sex))]
    public Sex? Sex { get; init; }

    [Required]
    public CountryRequest? Country { get; init; }

    [StringLength(500)]
    public string Picture { get; init; } = "";

    [Required]
    public PlayerDataRequest? Data { get; init; }

    public NewPlayer ToNewPlayer() => new(
        FirstName!, LastName!, ShortName!, Sex!.Value, Country!.Code!, Country.Picture, Picture,
        Data!.Rank!.Value, Data.Points!.Value, Data.Weight!.Value, Data.Height!.Value, Data.Age!.Value, Data.Last!);
}