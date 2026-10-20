namespace Tenisu.Infrastructure.Seeding;

internal sealed record PlayerJsonObject(int Id, string Firstname, string Lastname, string Shortname, string Sex, CountryJsonObject Country, string Picture, PlayerDataJsonObject Data);