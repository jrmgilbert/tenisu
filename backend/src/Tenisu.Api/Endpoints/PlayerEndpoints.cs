using Microsoft.AspNetCore.Http.HttpResults;

using Tenisu.Application.Players;

namespace Tenisu.Api.Endpoints;

public static class PlayerEndpoints
{
    public static IEndpointRouteBuilder MapPlayerEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/players").WithTags("Players");

        group.MapGet("/{id:int}", GetPlayerAsync)
             .WithName("GetPlayer")
             .WithSummary("One player by id");

        return app;
    }

    private static async Task<Results<Ok<PlayerDto>, NotFound>> GetPlayerAsync(int id, PlayerQueries queries, CancellationToken ct) =>
        await queries.GetAsync(id, ct) is { } player ? TypedResults.Ok(player) : TypedResults.NotFound();
}