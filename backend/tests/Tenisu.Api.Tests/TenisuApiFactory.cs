using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Tenisu.Api.Tests;

public class TenisuApiFactory : WebApplicationFactory<Program>
{
    protected virtual string DataFile => "TestData/headtohead.json";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.UseSetting("TenisuData:FilePath", DataFile);
        builder.UseSetting("TenisuData:DatabaseName", $"tenisu-test-{Guid.NewGuid():N}");
    }
}