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
}