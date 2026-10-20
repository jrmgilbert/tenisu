using System.ComponentModel.DataAnnotations;

using Tenisu.Core.Players;

namespace Tenisu.Api.Endpoints;

public sealed record CountryRequest
{
    [Required, RegularExpression(Country.CodePattern, ErrorMessage = "Use 3 capital letters, e.g. SRB.")]
    public string? Code { get; init; }

    [StringLength(500)]
    public string Picture { get; init; } = "";
}