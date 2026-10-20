using System.Net;
using System.Text.Json;

namespace Tenisu.Api.Tests;

public class ListPlayersEndpointTests(TenisuApiFactory api, EmptyTenisuApiFactory empty)
    : IClassFixture<TenisuApiFactory>, IClassFixture<EmptyTenisuApiFactory>
{
    [Fact]
    public async Task Returns_200_with_all_players()
    {
        var json = await ApiAssert.OkJsonAsync(await api.CreateClient().GetAsync("/api/players", TestContext.Current.CancellationToken));

        Assert.Equal(JsonValueKind.Array, json.ValueKind);
        Assert.Equal(5, json.GetArrayLength());
    }

    [Theory]
    [InlineData("/api/players")]
    [InlineData("/api/players?sort=BestFirst")]
    public async Task Sorts_best_first_by_default_or_when_asked(string url)
    {
        var json = await ApiAssert.OkJsonAsync(await api.CreateClient().GetAsync(url, TestContext.Current.CancellationToken));

        Assert.Equal([17, 52, 102, 65, 95], json.EnumerateArray().Select(p => p.GetProperty("id").GetInt32()));
    }

    [Theory]
    [InlineData("unknown")]
    [InlineData("42")]
    public async Task An_unknown_sort_returns_400_problem(string sort) =>
        ApiAssert.IsProblem(await api.CreateClient().GetAsync($"/api/players?sort={sort}", TestContext.Current.CancellationToken), HttpStatusCode.BadRequest);

    [Theory]
    [InlineData("br")]
    [InlineData("gzip")]
    public async Task Compresses_the_list_when_the_client_accepts_it(string encoding)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/players");
        request.Headers.AcceptEncoding.ParseAdd(encoding);

        var response = await api.CreateClient().SendAsync(request, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal([encoding], response.Content.Headers.ContentEncoding);
    }

    [Fact]
    public async Task Returns_an_empty_array_not_404_when_there_are_no_players()
    {
        var json = await ApiAssert.OkJsonAsync(await empty.CreateClient().GetAsync("/api/players", TestContext.Current.CancellationToken));

        Assert.Equal(0, json.GetArrayLength());
    }
}