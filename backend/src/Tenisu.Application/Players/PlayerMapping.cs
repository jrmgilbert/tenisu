using Tenisu.Core.Players;

namespace Tenisu.Application.Players;

internal static class PlayerMapping
{
    public static PlayerDto ToDto(this Player p) => new(
        p.Id, p.FirstName, p.LastName, p.ShortName, p.Sex,
        new CountryDto(p.Country.Code, p.Country.Picture),
        p.Picture,
        new PlayerDataDto(p.Data.Rank, p.Data.Points, p.Data.Weight, p.Data.Height, p.Data.Age, p.Data.Last.AsNumbers()));
}