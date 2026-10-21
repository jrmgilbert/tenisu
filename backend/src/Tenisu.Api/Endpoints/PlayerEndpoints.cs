using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using Tenisu.Api.Requests.Players;
using Tenisu.Application.Players;

namespace Tenisu.Api.Endpoints;

public static class PlayerEndpoints
{
    public const int MaxAddPlayerBodyBytes = 16 * 1024;

    public static IEndpointRouteBuilder MapPlayerEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/players").WithTags("Players");

        group.MapGet("/", ListPlayersAsync)
             .WithName(RouteNames.ListPlayers)
             .WithSummary("List players (best first)")
             .ProducesValidationProblem();

        group.MapGet("/statistics", GetStatisticsAsync)
             .WithName(RouteNames.GetStatistics)
             .WithSummary("Get players statistics")
             .WithDescription("Returns the country with the best win ratio, the average BMI and median height of all players");

        group.MapGet("/{id:int}", GetPlayerAsync)
             .WithName(RouteNames.GetPlayer)
             .WithSummary("Get a player")
             .WithDescription("Gets a player by its numerical ID");

        group.MapPost("/", AddPlayerAsync)
             .WithName(RouteNames.AddPlayer)
             .WithSummary("Add a player")
             .WithDescription($"At most {PlayerCommands.MaxPlayers} players in total and {RateLimitPolicies.PlayerWritesPerMinute} additions per minute per client. "
                            + "Players are kept in memory only: they are lost when the app restarts, is redeployed, "
                            + "or scales to zero after a few minutes without traffic.")
             .RequireRateLimiting(RateLimitPolicies.PlayerWrites)
             .WithMetadata(new RequestSizeLimitAttribute(MaxAddPlayerBodyBytes))
             .ProducesValidationProblem()
             .ProducesProblem(StatusCodes.Status409Conflict)
             .ProducesProblem(StatusCodes.Status413PayloadTooLarge)
             .ProducesProblem(StatusCodes.Status415UnsupportedMediaType)
             .ProducesProblem(StatusCodes.Status429TooManyRequests);

        return app;
    }

    private static async Task<Ok<IReadOnlyList<PlayerDto>>> ListPlayersAsync(PlayerQueries queries, CancellationToken ct,
        [EnumDataType(typeof(PlayerSort))] PlayerSort sort = PlayerSort.BestFirst) =>
        TypedResults.Ok(await queries.ListAsync(sort, ct));

    private static async Task<Ok<PlayerStatisticsDto>> GetStatisticsAsync(PlayerQueries queries, CancellationToken ct) =>
        TypedResults.Ok(await queries.GetStatisticsAsync(ct));

    private static async Task<Results<Ok<PlayerDto>, NotFound>> GetPlayerAsync(int id, PlayerQueries queries, CancellationToken ct) =>
        await queries.GetAsync(id, ct) is { } player ? TypedResults.Ok(player) : TypedResults.NotFound();

    private static async Task<CreatedAtRoute<PlayerDto>> AddPlayerAsync(AddPlayerRequest request, PlayerCommands commands, CancellationToken ct)
    {
        var player = await commands.AddAsync(request.ToNewPlayer(), ct);

        return TypedResults.CreatedAtRoute(player, RouteNames.GetPlayer, new { id = player.Id });
    }
}