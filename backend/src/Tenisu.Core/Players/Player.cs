using System.Text.RegularExpressions;

using Tenisu.Core.Common;

namespace Tenisu.Core.Players;

public sealed partial class Player
{
    public const string ShortNamePattern = @"^[A-Z]\.[A-Z]{3}$";

    public int Id { get; private set; }

    public string FirstName { get; private set; } = default!;

    public string LastName { get; private set; } = default!;

    public string ShortName { get; private set; } = default!;

    public Sex Sex { get; private set; }

    public Country Country { get; private set; } = default!;

    public string Picture { get; private set; } = default!;

    public PlayerData Data { get; private set; } = default!;

    private Player() { }

    public static Player Create(string firstName, string lastName, string shortName, Sex sex, Country country, string picture,
        PlayerData data, int id = 0)
    {
        if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
            throw new DomainException("First and last name are required.");

        var normalizedShortName = shortName.Trim().ToUpperInvariant();

        if (!ShortNameFormat().IsMatch(normalizedShortName))
            throw new DomainException($"'{shortName}' is not a valid short name: use an initial, a dot and 3 letters, e.g. N.DJO.");

        if (!Enum.IsDefined(sex))
            throw new DomainException($"'{sex}' is not a valid sex.");

        return new()
        {
            Id = id,
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            ShortName = normalizedShortName,
            Sex = sex,
            Country = country,
            Picture = picture,
            Data = data
        };
    }

    [GeneratedRegex(ShortNamePattern)]
    private static partial Regex ShortNameFormat();
}