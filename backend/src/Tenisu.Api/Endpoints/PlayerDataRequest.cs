using System.ComponentModel.DataAnnotations;

using Tenisu.Core.Players;

namespace Tenisu.Api.Endpoints;

public sealed record PlayerDataRequest : IValidatableObject
{
    [Required, Range(1, 10_000)]
    public int? Rank { get; init; }

    [Required, Range(0, 100_000)]
    public int? Points { get; init; }

    [Required, Range(30_000, 200_000)]
    public int? Weight { get; init; }

    [Required, Range(120, 250)]
    public int? Height { get; init; }

    [Required, Range(14, 60)]
    public int? Age { get; init; }

    [Required, MaxLength(LastResults.MaxResults)]
    public int[]? Last { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Last is not null && Last.Any(r => r is not (0 or 1)))
            yield return new ValidationResult("Each result must be 1 (win) or 0 (loss).", [nameof(Last)]);
    }
}