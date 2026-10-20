namespace Tenisu.Api.Tests;

public sealed class FullTenisuApiFactory : TenisuApiFactory
{
    protected override string DataFile => "TestData/full.json";
}