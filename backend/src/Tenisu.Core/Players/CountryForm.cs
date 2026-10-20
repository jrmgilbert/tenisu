namespace Tenisu.Core.Players;

internal sealed record CountryForm(string Code, int Wins, int Played)
{
    public double WinRatio => (double)Wins / Played;
}