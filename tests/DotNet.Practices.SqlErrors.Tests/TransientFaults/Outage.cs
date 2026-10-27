using DotNet.Practices.SqlErrors.Tests.Infrastructure;
using Microsoft.Data.SqlClient;

namespace DotNet.Practices.SqlErrors.Tests.TransientFaults;

// The local instance cannot go away like an Azure SQL gateway does, so the stored procedure the readers
// call raises the real Azure error number itself for the first N calls, then answers normally.
internal static class Outage
{
    public const string Schema = """
        CREATE TABLE LeaveBalances (EmployeeId int PRIMARY KEY, DaysLeft int NOT NULL);
        INSERT INTO LeaveBalances VALUES (1, 22);
        CREATE TABLE Outages (FailuresLeft int NOT NULL, Transient bit NOT NULL);
        INSERT INTO Outages VALUES (0, 1);
        """;

    public const string Procedure = """
        CREATE PROCEDURE GetDaysLeft @employeeId int AS
        BEGIN
            IF (SELECT FailuresLeft FROM Outages) > 0
            BEGIN
                UPDATE Outages SET FailuresLeft -= 1;
                IF (SELECT Transient FROM Outages) = 1
                    RAISERROR(40613, 16, 1, 2, 'db', 3, 'srv', 5, 'trace');
                ELSE
                    RAISERROR('Permanent failure', 16, 1);
                RETURN;
            END
            SELECT DaysLeft FROM LeaveBalances WHERE EmployeeId = @employeeId;
        END
        """;

    public static SqlServerTestDatabase CreateDatabase(int failures, bool transient = true)
    {
        var database = SqlServerTestDatabase.Create(Schema);
        database.Execute(Procedure);
        database.Execute($"UPDATE Outages SET FailuresLeft = {failures}, Transient = {(transient ? 1 : 0)};");
        return database;
    }

    public static int FailuresLeft(SqlServerTestDatabase database)
    {
        using var connection = new SqlConnection(database.ConnectionString);
        connection.Open();
        using var command = new SqlCommand("SELECT FailuresLeft FROM Outages;", connection);
        return (int)(command.ExecuteScalar() ?? 0);
    }
}
