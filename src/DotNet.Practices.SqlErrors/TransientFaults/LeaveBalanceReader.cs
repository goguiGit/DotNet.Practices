using System.Data;

namespace DotNet.Practices.SqlErrors.TransientFaults;

public sealed class LeaveBalanceReader(string connectionString, int maxAttempts, TimeSpan backoff)
{
    public async Task<int> GetDaysLeftAsync(int employeeId, CancellationToken cancellationToken = default)
    {
        for (var attempt = 1; ; attempt++)
        {
            try
            {
                return await ReadAsync(employeeId, cancellationToken);
            }
            catch (SqlException exception) when (TransientSqlErrors.Contains(exception.Number) && attempt < maxAttempts)
            {
                await Task.Delay(backoff * Math.Pow(2, attempt - 1), cancellationToken);
            }
        }
    }

    private async Task<int> ReadAsync(int employeeId, CancellationToken cancellationToken)
    {
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);
        await using var command = new SqlCommand("GetDaysLeft", connection) { CommandType = CommandType.StoredProcedure };
        command.Parameters.Add("@employeeId", SqlDbType.Int).Value = employeeId;
        var days = await command.ExecuteScalarAsync(cancellationToken);
        return days is int value ? value : 0;
    }
}
