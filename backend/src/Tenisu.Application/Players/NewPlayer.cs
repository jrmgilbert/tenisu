using Tenisu.Core.Common;

namespace Tenisu.Application.Players;

public sealed record NewPlayer(string FirstName, string LastName, string ShortName, Sex Sex, string CountryCode, string CountryPicture,
    string Picture, int Rank, int Points, int Weight, int Height, int Age, IReadOnlyList<int> Last);