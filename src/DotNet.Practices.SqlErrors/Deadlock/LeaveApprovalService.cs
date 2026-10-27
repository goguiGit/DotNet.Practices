using System.Data;

namespace DotNet.Practices.SqlErrors.Deadlock;

public sealed class LeaveApprovalService(string connectionString, int maxAttempts = 3)
{
    private const int DeadlockVictim = 1205;

    public async Task<Approval> ApproveAsync(int requestId, CancellationToken cancellationToken = default)
    {
        for (var attempt = 1; ; attempt++)
        {
            try
            {
                await ApproveOnceAsync(requestId, cancellationToken);
                return new Approval(requestId, attempt);
            }
            // The server already rolled the victim's transaction back, so retrying from scratch is safe.
            catch (SqlException exception) when (exception.Number == DeadlockVictim && attempt < maxAttempts)
            {
            }
        }
    }

    private async Task ApproveOnceAsync(int requestId, CancellationToken cancellationToken)
    {
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);
        await using var transaction = connection.BeginTransaction();

        await using var approve = new SqlCommand(
            "UPDATE LeaveRequests SET Status = 'Approved' OUTPUT inserted.EmployeeId, inserted.Days WHERE Id = @requestId AND Status = 'Pending';",
            connection,
            transaction);
        approve.Parameters.Add("@requestId", SqlDbType.Int).Value = requestId;
        var (employeeId, days) = await ReadApprovedRequestAsync(approve, cancellationToken);

        await using var subtract = new SqlCommand(
            "UPDATE LeaveBalances SET DaysLeft = DaysLeft - @days WHERE EmployeeId = @employeeId;",
            connection,
            transaction);
        subtract.Parameters.Add("@days", SqlDbType.Int).Value = days;
        subtract.Parameters.Add("@employeeId", SqlDbType.Int).Value = employeeId;
        await subtract.ExecuteNonQueryAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);
    }

    private static async Task<(int EmployeeId, int Days)> ReadApprovedRequestAsync(SqlCommand approve, CancellationToken cancellationToken)
    {
        await using var reader = await approve.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            throw new InvalidOperationException($"Leave request {approve.Parameters["@requestId"].Value} is not pending.");
        }

        return (reader.GetInt32(0), reader.GetInt32(1));
    }
}
