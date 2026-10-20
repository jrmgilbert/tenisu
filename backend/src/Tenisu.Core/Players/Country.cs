using System.Text.RegularExpressions;

using Tenisu.Core.Common;

namespace Tenisu.Core.Players;

public sealed partial record Country
{
    public const string CodePattern = "^[A-Z]{3}$";

    public string Code { get; private init; } = default!;

    public string Picture { get; private init; } = default!;

    private Country() { }

    public static Country Create(string code, string picture)
    {
        if (!CodeFormat().IsMatch(code))
            throw new DomainException($"'{code}' is not a valid country code: use 3 capital letters, e.g. SRB.");

        return new() { Code = code, Picture = picture };
    }

    [GeneratedRegex(CodePattern)]
    private static partial Regex CodeFormat();
}