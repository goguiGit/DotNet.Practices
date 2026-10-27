using System.Data;

namespace DotNet.Practices.SqlErrors.Timeout;

public sealed class AttendanceReportQuery(string connectionString, TimeSpan commandTimeout)
{
    public async Task<int> CountDaysWorkedAsync(int employeeId, DateOnly month, CancellationToken cancellationToken = default)
    {
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);
        await using var command = new SqlCommand(
            "SELECT COUNT(*) FROM ClockIns WHERE EmployeeId = @employeeId AND Day >= @from AND Day < @to;",
            connection);
        command.CommandTimeout = (int)commandTimeout.TotalSeconds;
        command.Parameters.Add("@employeeId", SqlDbType.Int).Value = employeeId;
        command.Parameters.Add("@from", SqlDbType.Date).Value = month;
        command.Parameters.Add("@to", SqlDbType.Date).Value = month.AddMonths(1);

        try
        {
            var count = await command.ExecuteScalarAsync(cancellationToken);
            return count is int days ? days : 0;
        }
        // SqlClient reports a cancelled command as SqlException ("Operation cancelled by user" / "A severe error
        // occurred"), not as OperationCanceledException; translate so callers see the standard cancellation.
        catch (SqlException) when (cancellationToken.IsCancellationRequested)
        {
            throw new OperationCanceledException(cancellationToken);
        }
    }
}
