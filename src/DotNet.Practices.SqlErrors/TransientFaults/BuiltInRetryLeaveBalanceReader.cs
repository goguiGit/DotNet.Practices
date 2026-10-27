using System.Data;

namespace DotNet.Practices.SqlErrors.TransientFaults;

public sealed class BuiltInRetryLeaveBalanceReader
{
    private readonly string _connectionString;
    private readonly SqlRetryLogicBaseProvider _retryProvider;

    public BuiltInRetryLeaveBalanceReader(string connectionString, int maxAttempts, TimeSpan backoff)
    {
        _connectionString = connectionString;
        _retryProvider = SqlConfigurableRetryFactory.CreateExponentialRetryProvider(new SqlRetryLogicOption
        {
            NumberOfTries = maxAttempts,
            DeltaTime = backoff,
            MaxTimeInterval = TimeSpan.FromSeconds(2),
            TransientErrors = TransientSqlErrors.Numbers,
        });
    }

    public async Task<int> GetDaysLeftAsync(int employeeId, CancellationToken cancellationToken = default)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        await using var command = new SqlCommand("GetDaysLeft", connection)
        {
            CommandType = CommandType.StoredProcedure,
            RetryLogicProvider = _retryProvider,
        };
        command.Parameters.Add("@employeeId", SqlDbType.Int).Value = employeeId;
        var days = await command.ExecuteScalarAsync(cancellationToken);
        return days is int value ? value : 0;
    }
}
