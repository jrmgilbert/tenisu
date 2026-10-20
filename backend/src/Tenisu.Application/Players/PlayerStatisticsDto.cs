namespace Tenisu.Application.Players;

public sealed record PlayerStatisticsDto(string? BestCountry, double? BestCountryWinRatio, double? AverageBmi, double? MedianHeight);