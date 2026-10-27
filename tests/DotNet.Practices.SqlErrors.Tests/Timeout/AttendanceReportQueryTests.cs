using System.Diagnostics;
using DotNet.Practices.SqlErrors.Tests.Infrastructure;
using DotNet.Practices.SqlErrors.Timeout;
using Microsoft.Data.SqlClient;

namespace DotNet.Practices.SqlErrors.Tests.Timeout;

[Category("Integration")]
public class AttendanceReportQueryTests
{
    private const string Schema = """
        CREATE TABLE ClockIns (EmployeeId int NOT NULL, Day date NOT NULL);
        INSERT INTO ClockIns VALUES (1, '2026-10-01'), (1, '2026-10-02'), (2, '2026-10-01');
        """;

    private static readonly DateOnly October = new(2026, 10, 1);
    private static readonly TimeSpan OneSecond = TimeSpan.FromSeconds(1);

    [Test]
    public async Task AttendanceReportQuery_CountDaysWorkedAsync_ReturnsDaysOfTheMonth()
    {
        using var database = SqlServerTestDatabase.Create(Schema);
        var query = new AttendanceReportQuery(database.ConnectionString, OneSecond);

        var days = await query.CountDaysWorkedAsync(employeeId: 1, October);

        Assert.That(days, Is.EqualTo(2));
    }

    [Test]
    public async Task AttendanceReportQuery_CountDaysWorkedAsync_BlockedRowsFailWithTimeoutNumber()
    {
        using var database = SqlServerTestDatabase.Create(Schema);
        await using var blocker = await HoldRowsLockedAsync(database);
        var query = new AttendanceReportQuery(database.ConnectionString, OneSecond);

        var exception = Assert.ThrowsAsync<SqlException>(() => query.CountDaysWorkedAsync(employeeId: 1, October));

        Assert.That(exception.Number, Is.EqualTo(-2));
    }

    [Test]
    public async Task AttendanceReportQuery_CountDaysWorkedAsync_BlockedRowsGiveUpAfterTheCommandTimeout()
    {
        using var database = SqlServerTestDatabase.Create(Schema);
        await using var blocker = await HoldRowsLockedAsync(database);
        var query = new AttendanceReportQuery(database.ConnectionString, OneSecond);
        var stopwatch = Stopwatch.StartNew();

        Assert.ThrowsAsync<SqlException>(() => query.CountDaysWorkedAsync(employeeId: 1, October));

        Assert.That(stopwatch.Elapsed, Is.LessThan(TimeSpan.FromSeconds(10)));
    }

    [Test]
    public async Task AttendanceReportQuery_CountDaysWorkedAsync_CancelledCallerStopsWaiting()
    {
        using var database = SqlServerTestDatabase.Create(Schema);
        await using var blocker = await HoldRowsLockedAsync(database);
        var query = new AttendanceReportQuery(database.ConnectionString, TimeSpan.FromSeconds(30));
        using var cancellation = new CancellationTokenSource(TimeSpan.FromMilliseconds(200));

        Assert.ThrowsAsync<OperationCanceledException>(() => query.CountDaysWorkedAsync(employeeId: 1, October, cancellation.Token));
    }

    private static async Task<SqlConnection> HoldRowsLockedAsync(SqlServerTestDatabase database)
    {
        var connection = new SqlConnection(database.ConnectionString);
        await connection.OpenAsync();
        var transaction = connection.BeginTransaction();
        await using var update = new SqlCommand("SELECT 1 FROM ClockIns WITH (XLOCK, HOLDLOCK) WHERE EmployeeId = 1;", connection, transaction);
        await update.ExecuteNonQueryAsync();
        return connection;
    }
}
