using DotNet.Practices.SqlErrors.Tests.Infrastructure;
using Microsoft.Data.SqlClient;

namespace DotNet.Practices.SqlErrors.Tests.NPlusOne;

// Counts the round trips (text batches and RPC calls such as sp_executesql) the server completed against the test
// database while the action ran, with an Extended Events session whose event_counter target updates synchronously.
// The reports under test need no instrumentation. sys.dm_exec_query_stats was tried first: it publishes late.
internal static class QueryCounter
{
    public static async Task<long> CountAsync(SqlServerTestDatabase database, Func<Task> action)
    {
        var session = $"QueryCounter_{database.Name}";
        var master = new SqlConnectionStringBuilder(database.ConnectionString) { InitialCatalog = "master" }.ConnectionString;
        await using var connection = new SqlConnection(master);
        await connection.OpenAsync();
        await ExecuteAsync(connection, $"""
            DECLARE @db nvarchar(10) = CAST(DB_ID('{database.Name}') AS nvarchar(10));
            DECLARE @sql nvarchar(max) = N'CREATE EVENT SESSION [{session}] ON SERVER
                ADD EVENT sqlserver.sql_batch_completed (WHERE sqlserver.database_id = ' + @db + N'),
                ADD EVENT sqlserver.rpc_completed (WHERE sqlserver.database_id = ' + @db + N')
                ADD TARGET package0.event_counter;';
            EXEC sp_executesql @sql;
            ALTER EVENT SESSION [{session}] ON SERVER STATE = START;
            """);
        try
        {
            await action();
            return await CompletedStatementsAsync(connection, session);
        }
        finally
        {
            await ExecuteAsync(connection, $"DROP EVENT SESSION [{session}] ON SERVER;");
        }
    }

    private static async Task<long> CompletedStatementsAsync(SqlConnection connection, string session)
    {
        await using var command = new SqlCommand("""
            SELECT ISNULL(CAST(targets.target_data AS xml).value('(CounterTarget/Packages/Package/Event/@count)[1]', 'bigint'), 0)
            FROM sys.dm_xe_sessions sessions
            JOIN sys.dm_xe_session_targets targets ON targets.event_session_address = sessions.address
            WHERE sessions.name = @session AND targets.target_name = 'event_counter';
            """, connection);
        command.Parameters.AddWithValue("@session", session);
        return Convert.ToInt64(await command.ExecuteScalarAsync());
    }

    private static async Task ExecuteAsync(SqlConnection connection, string sql)
    {
        await using var command = new SqlCommand(sql, connection);
        await command.ExecuteNonQueryAsync();
    }
}
