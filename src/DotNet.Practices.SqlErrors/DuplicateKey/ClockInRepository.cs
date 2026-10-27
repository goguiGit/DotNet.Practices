using System.Data;

namespace DotNet.Practices.SqlErrors.DuplicateKey;

public sealed class ClockInRepository(string connectionString)
{
    // 2627: PRIMARY KEY or UNIQUE constraint; 2601: unique index. Same business meaning, two numbers.
    private const int UniqueConstraintViolation = 2627;
    private const int UniqueIndexViolation = 2601;

    public async Task<ClockInResult> InsertAsync(int employeeId, DateOnly day, DateTime clockedAt, CancellationToken cancellationToken = default)
    {
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);
        await using var command = new SqlCommand(
            "INSERT INTO ClockIns (EmployeeId, Day, ClockedAt) VALUES (@employeeId, @day, @clockedAt);",
            connection);
        command.Parameters.Add("@employeeId", SqlDbType.Int).Value = employeeId;
        command.Parameters.Add("@day", SqlDbType.Date).Value = day;
        command.Parameters.Add("@clockedAt", SqlDbType.DateTime2).Value = clockedAt;

        try
        {
            await command.ExecuteNonQueryAsync(cancellationToken);
            return ClockInResult.Inserted;
        }
        catch (SqlException exception) when (exception.Number is UniqueConstraintViolation or UniqueIndexViolation)
        {
            return ClockInResult.AlreadyExists;
        }
    }
}
