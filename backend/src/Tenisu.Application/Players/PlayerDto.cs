using Tenisu.Core.Common;

namespace Tenisu.Application.Players;

public sealed record PlayerDto(int Id, string FirstName, string LastName, string ShortName, Sex Sex, CountryDto Country, string Picture, PlayerDataDto Data);