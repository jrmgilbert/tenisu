using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace Tenisu.Api.Tests;

internal static class ApiAssert
{
    public static async Task<JsonElement> OkJsonAsync(HttpResponseMessage response)
    {
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);

        return await response.Content.ReadFromJsonAsync<JsonElement>();
    }

    public static void IsProblem(HttpResponseMessage response, HttpStatusCode status)
    {
        Assert.Equal(status, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    public static async Task HasValidationErrorAsync(HttpResponseMessage response, string field)
    {
        IsProblem(response, HttpStatusCode.BadRequest);

        var errors = (await response.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("errors");

        Assert.Contains(errors.EnumerateObject(), e => Leaf(e.Name).Equals(Leaf(field), StringComparison.OrdinalIgnoreCase));
    }

    private static string Leaf(string path) => path[(path.LastIndexOf('.') + 1)..];
}