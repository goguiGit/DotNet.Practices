using DotNet.Practices.SqlErrors.TransientFaults;
using Microsoft.Data.SqlClient;

namespace DotNet.Practices.SqlErrors.Tests.TransientFaults;

[Category("Integration")]
public class LeaveBalanceReaderTests
{
    private static readonly TimeSpan ShortBackoff = TimeSpan.FromMilliseconds(10);

    [Test]
    public async Task LeaveBalanceReader_GetDaysLeftAsync_ReturnsTheBalance()
    {
        using var database = Outage.CreateDatabase(failures: 0);
        var reader = new LeaveBalanceReader(database.ConnectionString, maxAttempts: 3, ShortBackoff);

        var days = await reader.GetDaysLeftAsync(employeeId: 1);

        Assert.That(days, Is.EqualTo(22));
    }

    [Test]
    public async Task LeaveBalanceReader_GetDaysLeftAsync_RecoversFromTwoTransientFailures()
    {
        using var database = Outage.CreateDatabase(failures: 2);
        var reader = new LeaveBalanceReader(database.ConnectionString, maxAttempts: 3, ShortBackoff);

        var days = await reader.GetDaysLeftAsync(employeeId: 1);

        Assert.That(days, Is.EqualTo(22));
    }

    [Test]
    public void LeaveBalanceReader_GetDaysLeftAsync_OutOfAttemptsPropagatesTheTransientNumber()
    {
        using var database = Outage.CreateDatabase(failures: 3);
        var reader = new LeaveBalanceReader(database.ConnectionString, maxAttempts: 3, ShortBackoff);

        var exception = Assert.ThrowsAsync<SqlException>(() => reader.GetDaysLeftAsync(employeeId: 1));

        Assert.That(exception.Number, Is.EqualTo(40613));
    }

    [Test]
    public void LeaveBalanceReader_GetDaysLeftAsync_NonTransientErrorPropagatesAtOnce()
    {
        using var database = Outage.CreateDatabase(failures: 2, transient: false);
        var reader = new LeaveBalanceReader(database.ConnectionString, maxAttempts: 3, ShortBackoff);

        var exception = Assert.ThrowsAsync<SqlException>(() => reader.GetDaysLeftAsync(employeeId: 1));

        Assert.That(exception.Number, Is.EqualTo(50000));
    }

    [Test]
    public void LeaveBalanceReader_GetDaysLeftAsync_NonTransientErrorIsNotRetried()
    {
        using var database = Outage.CreateDatabase(failures: 2, transient: false);
        var reader = new LeaveBalanceReader(database.ConnectionString, maxAttempts: 3, ShortBackoff);

        Assert.ThrowsAsync<SqlException>(() => reader.GetDaysLeftAsync(employeeId: 1));

        Assert.That(Outage.FailuresLeft(database), Is.EqualTo(1));
    }
}
