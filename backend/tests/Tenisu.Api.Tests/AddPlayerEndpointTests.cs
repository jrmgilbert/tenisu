using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

using Tenisu.Core.Players;

namespace Tenisu.Api.Tests;

public sealed class AddPlayerEndpointTests : IDisposable
{
    private readonly TenisuApiFactory _api = new();
    private readonly HttpClient _client;

    public AddPlayerEndpointTests() => _client = _api.CreateClient();

    public void Dispose()
    {
        _client.Dispose();
        _api.Dispose();
    }

    [Fact]
    public async Task A_valid_body_returns_201_with_location_and_the_player()
    {
        var response = await PostAsync(ValidBody());

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var json = await response.Content.ReadFromJsonAsync<JsonElement>(TestContext.Current.CancellationToken);
        var id = json.GetProperty("id").GetInt32();
        Assert.EndsWith($"/api/players/{id}", response.Headers.Location!.ToString());
        Assert.Equal("E.ECH", json.GetProperty("shortName").GetString());
        Assert.Equal([1, 0, 1, 1, 0], json.GetProperty("data").GetProperty("last").EnumerateArray().Select(r => r.GetInt32()));
    }

    [Fact]
    public async Task The_location_serves_the_created_player()
    {
        var created = await PostAsync(ValidBody());

        var json = await ApiAssert.OkJsonAsync(await _client.GetAsync(created.Headers.Location, TestContext.Current.CancellationToken));

        Assert.Equal("E.ECH", json.GetProperty("shortName").GetString());
    }

    [Fact]
    public async Task The_data_file_casing_is_accepted_too()
    {
        var body = ValidBody();
        foreach (var (domainName, fileName) in new[] { ("firstName", "firstname"), ("lastName", "lastname"), ("shortName", "shortname") })
        {
            body[fileName] = body[domainName]!.DeepClone();
            body.Remove(domainName);
        }

        Assert.Equal(HttpStatusCode.Created, (await PostAsync(body)).StatusCode);
    }

    [Theory]
    [InlineData("firstName")]
    [InlineData("lastName")]
    [InlineData("shortName")]
    [InlineData("sex")]
    [InlineData("country")]
    [InlineData("country.code")]
    [InlineData("data")]
    [InlineData("data.rank")]
    [InlineData("data.points")]
    [InlineData("data.weight")]
    [InlineData("data.height")]
    [InlineData("data.age")]
    [InlineData("data.last")]
    public async Task A_missing_required_field_returns_400(string path)
    {
        var body = ValidBody();
        Parent(body, path, out var leaf).Remove(leaf);

        await ApiAssert.HasValidationErrorAsync(await PostAsync(body), path);
    }

    [Theory]
    [InlineData("firstName", "\"\"")]
    [InlineData("shortName", "\"NECH\"")]
    [InlineData("shortName", "\"e.ech\"")]
    [InlineData("sex", "7")]
    [InlineData("country.code", "\"srb\"")]
    [InlineData("country.code", "\"FRAN\"")]
    [InlineData("data.rank", "0")]
    [InlineData("data.points", "-1")]
    [InlineData("data.weight", "10")]
    [InlineData("data.height", "300")]
    [InlineData("data.age", "5")]
    [InlineData("data.last", "[1, 2]")]
    public async Task An_invalid_value_returns_400(string path, string jsonValue)
    {
        var body = ValidBody();
        Parent(body, path, out var leaf)[leaf] = JsonNode.Parse(jsonValue);

        await ApiAssert.HasValidationErrorAsync(await PostAsync(body), path);
    }

    [Fact]
    public async Task Too_many_last_results_return_400()
    {
        var body = ValidBody();
        body["data"]!["last"] = JsonSerializer.SerializeToNode(Enumerable.Repeat(1, LastResults.MaxResults + 1));

        await ApiAssert.HasValidationErrorAsync(await PostAsync(body), "data.last");
    }

    [Fact]
    public async Task A_blank_name_returns_400()
    {
        var body = ValidBody();
        body["firstName"] = " ";

        await ApiAssert.HasValidationErrorAsync(await PostAsync(body), "firstName");
    }

    [Fact]
    public async Task An_unknown_sex_returns_400()
    {
        var body = ValidBody();
        body["sex"] = "X";

        Assert.Equal(HttpStatusCode.BadRequest, (await PostAsync(body)).StatusCode);
    }

    [Fact]
    public async Task A_taken_short_name_returns_409_problem()
    {
        var body = ValidBody();
        body["shortName"] = "N.DJO";

        ApiAssert.IsProblem(await PostAsync(body), HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task A_non_json_body_returns_415()
    {
        using var content = new StringContent("firstName=Emma", Encoding.UTF8, "text/plain");

        var response = await _client.PostAsync("/api/players", content, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.UnsupportedMediaType, response.StatusCode);
    }

    [Fact]
    public async Task A_client_supplied_id_is_ignored()
    {
        var body = ValidBody();
        body["id"] = 999;

        var response = await PostAsync(body);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var json = await response.Content.ReadFromJsonAsync<JsonElement>(TestContext.Current.CancellationToken);
        Assert.NotEqual(999, json.GetProperty("id").GetInt32());
    }

    private Task<HttpResponseMessage> PostAsync(JsonObject body) =>
        _client.PostAsJsonAsync("/api/players", body, TestContext.Current.CancellationToken);

    private static JsonObject ValidBody() => JsonNode.Parse("""
        {
          "firstName": "Emma", "lastName": "Echo", "shortName": "E.ECH", "sex": "F",
          "country": { "code": "ITA", "picture": "" }, "picture": "",
          "data": { "rank": 5, "points": 1200, "weight": 62000, "height": 172, "age": 24, "last": [1, 0, 1, 1, 0] }
        }
        """)!.AsObject();

    private static JsonObject Parent(JsonObject body, string path, out string leaf)
    {
        var parts = path.Split('.');
        leaf = parts[^1];

        var node = body;
        foreach (var part in parts[..^1])
            node = node[part]!.AsObject();

        return node;
    }
}