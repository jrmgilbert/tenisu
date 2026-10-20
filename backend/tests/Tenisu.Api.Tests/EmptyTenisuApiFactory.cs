namespace Tenisu.Api.Tests;

public sealed class EmptyTenisuApiFactory : TenisuApiFactory
{
    protected override string DataFile => "TestData/empty.json";
}