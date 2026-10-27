using Microsoft.Data.SqlClient;

namespace DotNet.Practices.SqlErrors.Tests.Infrastructure;

public sealed class SqlServerTestDatabase : IDisposable
{
    private const string ServerVariable = "DOTNET_PRACTICES_SQLSERVER";
    private const string DefaultServer = "Server=localhost;Integrated Security=true;TrustServerCertificate=true";

    private readonly string _serverConnectionString;

    private SqlServerTestDatabase(string serverConnectionString, string name)
    {
        _serverConnectionString = serverConnectionString;
        Name = name;
        ConnectionString = new SqlConnectionStringBuilder(serverConnectionString) { InitialCatalog = name }.ConnectionString;
    }

    public string Name { get; }

    public string ConnectionString { get; }

    public static SqlServerTestDatabase Create(string schema)
    {
        var server = Environment.GetEnvironmentVariable(ServerVariable) ?? DefaultServer;
        var name = $"DotNetPractices_SqlErrors_{Guid.NewGuid():N}";
        try
        {
            Execute(server, $"CREATE DATABASE [{name}]");
        }
        catch (SqlException exception)
        {
            Assert.Ignore($"SQL Server not reachable with '{server}' ({exception.Message}). Set {ServerVariable} to a connection string that can CREATE DATABASE.");
        }

        var database = new SqlServerTestDatabase(server, name);
        database.Execute(schema);
        return database;
    }

    public void Execute(string sql) => Execute(ConnectionString, sql);

    public void Dispose() =>
        Execute(_serverConnectionString, $"ALTER DATABASE [{Name}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{Name}];");

    private static void Execute(string connectionString, string sql)
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        using var command = new SqlCommand(sql, connection);
        command.ExecuteNonQuery();
    }
}
