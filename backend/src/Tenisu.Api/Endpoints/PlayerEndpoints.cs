using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Http.HttpResults;

using Tenisu.Api.Requests.Players;
using Tenisu.Application.Players;

namespace Tenisu.Api.Endpoints;

public static class PlayerEndpoints
{
    public static IEndpointRouteBuilder MapPlayerEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/players").WithTags("Players");

        group.MapGet("/", ListPlayersAsync)
             .WithName(RouteNames.ListPlayers)
             .WithSummary("List players (best first)")
             .ProducesValidationProblem();

        group.MapGet("/statistics", GetStatisticsAsync)
             .WithName(RouteNames.GetStatistics)
             .WithSummary("Get players statistics (country with the best win ratio, average BMI and median height)");

        group.MapGet("/{id:int}", GetPlayerAsync)
             .WithName(RouteNames.GetPlayer)
             .WithSummary("Get a player by his/her id");

        group.MapPost("/", AddPlayerAsync)
             .WithName(RouteNames.AddPlayer)
             .WithSummary("Add a player")
             .ProducesValidationProblem()
             .ProducesProblem(StatusCodes.Status409Conflict)
             .ProducesProblem(StatusCodes.Status415UnsupportedMediaType);

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