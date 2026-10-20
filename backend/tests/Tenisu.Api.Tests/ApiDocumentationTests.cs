using System.Net;

namespace Tenisu.Api.Tests;

public class ApiDocumentationTests(TenisuApiFactory api) : IClassFixture<TenisuApiFactory>
{
    [Theory]
    [InlineData("/openapi/v1.json")]
    [InlineData("/scalar")]
    public async Task Docs_are_served_outside_development(string url) =>
        Assert.Equal(HttpStatusCode.OK, (await api.CreateClient().GetAsync(url, TestContext.Current.CancellationToken)).StatusCode);
}