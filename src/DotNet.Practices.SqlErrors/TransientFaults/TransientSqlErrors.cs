namespace DotNet.Practices.SqlErrors.TransientFaults;

public static class TransientSqlErrors
{
    // Azure SQL numbers that mean "the service is not available right now, try again", as listed in
    // https://learn.microsoft.com/azure/azure-sql/database/troubleshoot-common-errors-issues.
    public static IReadOnlyCollection<int> Numbers { get; } =
    [
        4060,   // Cannot open database requested by the login (failover in progress)
        10928,  // Resource limit for the database has been reached
        10929,  // Server too busy to support requests greater than the minimum guarantee
        40197,  // Service error processing the request (hardware/software upgrade, failover)
        40501,  // Service is currently busy
        40613,  // Database is not currently available
        49918,  // Not enough resources to process request
        49919,  // Too many create or update operations in progress
        49920,  // Too many operations in progress for the subscription
    ];

    private static readonly HashSet<int> NumberSet = [.. Numbers];

    public static bool Contains(int number) => NumberSet.Contains(number);
}
