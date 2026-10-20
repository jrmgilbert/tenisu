using System.Net;

namespace Tenisu.Api.Tests;

public class GetPlayerEndpointTests(TenisuApiFactory api) : IClassFixture<TenisuApiFactory>
{
    [Fact]
    public async Task Returns_200_with_the_player()
    {
        var json = await ApiAssert.OkJsonAsync(await api.CreateClient().GetAsync("/api/players/52"));

        Assert.Equal(52, json.GetProperty("id").GetInt32());
        Assert.Equal("N.DJO", json.GetProperty("shortName").GetString());
        Assert.Equal("M", json.GetProperty("sex").GetString());
        Assert.Equal("SRB", json.GetProperty("country").GetProperty("code").GetString());
        Assert.Equal(80000, json.GetProperty("data").GetProperty("weight").GetInt32());
        Assert.Equal([1, 1, 1, 1, 1], json.GetProperty("data").GetProperty("last").EnumerateArray().Select(r => r.GetInt32()));
    }

    [Fact]
    public async Task Returns_404_problem_for_an_unknown_id() =>
        ApiAssert.IsProblem(await api.CreateClient().GetAsync("/api/players/999"), HttpStatusCode.NotFound);

    [Fact]
    public async Task Returns_404_for_a_non_numeric_id() =>
        Assert.Equal(HttpStatusCode.NotFound, (await api.CreateClient().GetAsync("/api/players/abc")).StatusCode);

    [Theory]
    [InlineData("PUT")]
    [InlineData("PATCH")]
    [InlineData("DELETE")]
    public async Task Other_methods_on_a_player_return_405(string method)
    {
        var response = await api.CreateClient().SendAsync(new HttpRequestMessage(new HttpMethod(method), "/api/players/52"));

        Assert.Equal(HttpStatusCode.MethodNotAllowed, response.StatusCode);
    }
}