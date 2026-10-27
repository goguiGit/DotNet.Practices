using DotNet.Practices.SqlErrors.TransientFaults;
using Microsoft.Data.SqlClient;

namespace DotNet.Practices.SqlErrors.Tests.TransientFaults;

[Category("Integration")]
public class BuiltInRetryLeaveBalanceReaderTests
{
    private static readonly TimeSpan ShortBackoff = TimeSpan.FromMilliseconds(10);

    [Test]
    public async Task BuiltInRetryLeaveBalanceReader_GetDaysLeftAsync_RecoversFromTwoTransientFailures()
    {
        using var database = Outage.CreateDatabase(failures: 2);
        var reader = new BuiltInRetryLeaveBalanceReader(database.ConnectionString, maxAttempts: 3, ShortBackoff);

        var days = await reader.GetDaysLeftAsync(employeeId: 1);

        Assert.That(days, Is.EqualTo(22));
    }

    // Unlike a hand-written loop, SqlClient wraps every failed attempt in an AggregateException.
    [Test]
    public void BuiltInRetryLeaveBalanceReader_GetDaysLeftAsync_OutOfAttemptsAggregatesEveryAttempt()
    {
        using var database = Outage.CreateDatabase(failures: 3);
        var reader = new BuiltInRetryLeaveBalanceReader(database.ConnectionString, maxAttempts: 3, ShortBackoff);

        var exception = Assert.ThrowsAsync<AggregateException>(() => reader.GetDaysLeftAsync(employeeId: 1));

        Assert.That(exception.InnerExceptions.Select(inner => ((SqlException)inner).Number), Is.EqualTo(new[] { 40613, 40613, 40613 }));
    }
}
