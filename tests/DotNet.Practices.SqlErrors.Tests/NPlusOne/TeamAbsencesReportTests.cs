using DotNet.Practices.SqlErrors.NPlusOne;
using DotNet.Practices.SqlErrors.Tests.Infrastructure;
using Microsoft.Data.SqlClient;

namespace DotNet.Practices.SqlErrors.Tests.NPlusOne;

[Category("Integration")]
public class TeamAbsencesReportTests
{
    private const string Schema = """
        CREATE TABLE Employees (Id int PRIMARY KEY, Name nvarchar(100) NOT NULL);
        CREATE TABLE Absences (Id int IDENTITY PRIMARY KEY, EmployeeId int NOT NULL REFERENCES Employees(Id), Day date NOT NULL);
        INSERT INTO Employees VALUES (1, N'Ana'), (2, N'Luis'), (3, N'Marta');
        INSERT INTO Absences (EmployeeId, Day) VALUES (1, '2026-10-05'), (1, '2026-10-06'), (3, '2026-10-20');
        """;

    private static readonly EmployeeAbsences[] Expected =
    [
        new("Ana", "2026-10-05, 2026-10-06"),
        new("Luis", ""),
        new("Marta", "2026-10-20"),
    ];

    [Test]
    public async Task TeamAbsencesReport_BuildAsync_ListsEveryEmployeeWithTheirAbsences()
    {
        using var database = SqlServerTestDatabase.Create(Schema);
        var report = new TeamAbsencesReport(database.ConnectionString);

        var rows = await report.BuildAsync();

        Assert.That(rows, Is.EqualTo(Expected));
    }

    [Test]
    public async Task TeamAbsencesReport_BuildAsync_RunsOneQueryRegardlessOfTheNumberOfEmployees()
    {
        using var database = SqlServerTestDatabase.Create(Schema);
        var report = new TeamAbsencesReport(database.ConnectionString);

        var queries = await QueryCounter.CountAsync(database, () => report.BuildAsync());

        Assert.That(queries, Is.EqualTo(1));
    }

    [Test]
    public async Task OneQueryPerEmployee_BuildAsync_ListsTheSameRows()
    {
        using var database = SqlServerTestDatabase.Create(Schema);
        var report = new OneQueryPerEmployeeReport(database.ConnectionString);

        var rows = await report.BuildAsync();

        Assert.That(rows, Is.EqualTo(Expected));
    }

    [Test]
    public async Task OneQueryPerEmployee_BuildAsync_RunsOneQueryPlusOnePerEmployee()
    {
        using var database = SqlServerTestDatabase.Create(Schema);
        var report = new OneQueryPerEmployeeReport(database.ConnectionString);

        var queries = await QueryCounter.CountAsync(database, () => report.BuildAsync());

        Assert.That(queries, Is.EqualTo(1 + 3));
    }
}
