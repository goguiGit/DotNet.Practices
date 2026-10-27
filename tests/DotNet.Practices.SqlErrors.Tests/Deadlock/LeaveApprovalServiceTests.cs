using DotNet.Practices.SqlErrors.Deadlock;
using DotNet.Practices.SqlErrors.Tests.Infrastructure;
using Microsoft.Data.SqlClient;

namespace DotNet.Practices.SqlErrors.Tests.Deadlock;

[Category("Integration")]
public class LeaveApprovalServiceTests
{
    private const string Schema = """
        CREATE TABLE LeaveRequests (Id int PRIMARY KEY, EmployeeId int NOT NULL, Days int NOT NULL, Status varchar(20) NOT NULL);
        CREATE TABLE LeaveBalances (EmployeeId int PRIMARY KEY, DaysLeft int NOT NULL);
        INSERT INTO LeaveRequests VALUES (1, 1, 3, 'Pending');
        INSERT INTO LeaveBalances VALUES (1, 22);
        """;

    [Test]
    public async Task LeaveApprovalService_ApproveAsync_WithoutContentionSucceedsOnTheFirstAttempt()
    {
        using var database = SqlServerTestDatabase.Create(Schema);
        var service = new LeaveApprovalService(database.ConnectionString);

        var approval = await service.ApproveAsync(requestId: 1);

        Assert.That(approval, Is.EqualTo(new Approval(RequestId: 1, Attempts: 1)));
    }

    [Test]
    public async Task LeaveApprovalService_ApproveAsync_SubtractsTheDaysFromTheBalance()
    {
        using var database = SqlServerTestDatabase.Create(Schema);
        var service = new LeaveApprovalService(database.ConnectionString);

        await service.ApproveAsync(requestId: 1);

        Assert.That(DaysLeft(database, employeeId: 1), Is.EqualTo(19));
    }

    [Test]
    public async Task LeaveApprovalService_ApproveAsync_DeadlockVictimRetriesAndSucceeds()
    {
        using var database = SqlServerTestDatabase.Create(Schema);
        var service = new LeaveApprovalService(database.ConnectionString);

        var approval = await ApproveWhileAnotherProcessLocksInReverseOrderAsync(database, service);

        Assert.That(approval, Is.EqualTo(new Approval(RequestId: 1, Attempts: 2)));
    }

    [Test]
    public async Task LeaveApprovalService_ApproveAsync_OutOfAttemptsLetsTheDeadlockNumberPropagate()
    {
        using var database = SqlServerTestDatabase.Create(Schema);
        var service = new LeaveApprovalService(database.ConnectionString, maxAttempts: 1);

        var exception = Assert.ThrowsAsync<SqlException>(() => ApproveWhileAnotherProcessLocksInReverseOrderAsync(database, service));

        Assert.That(exception.Number, Is.EqualTo(1205));
    }

    // Simulates a balance recalculation that locks LeaveBalances first and LeaveRequests second: the
    // opposite order to the service. DEADLOCK_PRIORITY HIGH makes the service the victim every time.
    private static async Task<Approval> ApproveWhileAnotherProcessLocksInReverseOrderAsync(SqlServerTestDatabase database, LeaveApprovalService service)
    {
        await using var other = new SqlConnection(database.ConnectionString);
        await other.OpenAsync();
        await Execute(other, "SET DEADLOCK_PRIORITY HIGH; BEGIN TRANSACTION; SELECT 1 FROM LeaveBalances WITH (XLOCK, HOLDLOCK) WHERE EmployeeId = 1;");

        var approving = service.ApproveAsync(requestId: 1);
        await WaitUntilBlockedAsync(database);

        await Execute(other, "SELECT 1 FROM LeaveRequests WITH (XLOCK, HOLDLOCK) WHERE Id = 1; COMMIT;");
        return await approving;
    }

    private static async Task WaitUntilBlockedAsync(SqlServerTestDatabase database)
    {
        await using var connection = new SqlConnection(database.ConnectionString);
        await connection.OpenAsync();
        await using var command = new SqlCommand("SELECT COUNT(*) FROM sys.dm_exec_requests WHERE blocking_session_id <> 0;", connection);
        var deadline = DateTime.UtcNow.AddSeconds(10);
        while ((int)(await command.ExecuteScalarAsync() ?? 0) == 0)
        {
            if (DateTime.UtcNow > deadline)
            {
                Assert.Fail("The service never blocked behind the other process.");
            }

            await Task.Delay(50);
        }
    }

    private static int DaysLeft(SqlServerTestDatabase database, int employeeId)
    {
        using var connection = new SqlConnection(database.ConnectionString);
        connection.Open();
        using var command = new SqlCommand($"SELECT DaysLeft FROM LeaveBalances WHERE EmployeeId = {employeeId};", connection);
        return (int)(command.ExecuteScalar() ?? 0);
    }

    private static async Task Execute(SqlConnection connection, string sql)
    {
        await using var command = new SqlCommand(sql, connection);
        await command.ExecuteNonQueryAsync();
    }
}
