using Microsoft.Extensions.Configuration;

namespace Tenisu.Infrastructure.Persistence;

internal sealed record TenisuDataSettings(string ConnectionString, string DataFilePath)
{
    public static TenisuDataSettings From(IConfiguration config) => new(
        $"Data Source={config["TenisuData:DatabaseName"] ?? "tenisu"};Mode=Memory;Cache=Shared",
        Path.Combine(AppContext.BaseDirectory, config["TenisuData:FilePath"] ?? "Data/headtohead.json"));
}