using Microsoft.Data.Sqlite;

namespace Tenisu.Infrastructure.Persistence;

internal sealed class SqliteConnectionManager : IDisposable
{
    private readonly SqliteConnection _connection;

    public SqliteConnectionManager(string connectionString)
    {
        _connection = new SqliteConnection(connectionString);
        _connection.Open();
    }

    public void Dispose() => _connection.Dispose();
}