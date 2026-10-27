using DotNet.Practices.SqlErrors.DuplicateKey;
using DotNet.Practices.SqlErrors.Tests.Infrastructure;

namespace DotNet.Practices.SqlErrors.Tests.DuplicateKey;

[Category("Integration")]
public class ClockInRepositoryTests
{
    private const string Schema = """
        CREATE TABLE ClockIns (
            EmployeeId int NOT NULL,
            Day date NOT NULL,
            ClockedAt datetime2 NOT NULL,
            CONSTRAINT UQ_ClockIns_Employee_Day UNIQUE (EmployeeId, Day)
        );
        """;

    private static readonly DateOnly Monday = new(2026, 10, 26);

    [Test]
    public async Task ClockInRepository_InsertAsync_FirstClockInOfTheDayIsInserted()
    {
        using var database = SqlServerTestDatabase.Create(Schema);
        var repository = new ClockInRepository(database.ConnectionString);

        var result = await repository.InsertAsync(employeeId: 1, Monday, Monday.ToDateTime(new TimeOnly(9, 0)));

        Assert.That(result, Is.EqualTo(ClockInResult.Inserted));
    }

    [Test]
    public async Task ClockInRepository_InsertAsync_SecondClockInSameDayAlreadyExists()
    {
        using var database = SqlServerTestDatabase.Create(Schema);
        var repository = new ClockInRepository(database.ConnectionString);
        await repository.InsertAsync(employeeId: 1, Monday, Monday.ToDateTime(new TimeOnly(9, 0)));

        var result = await repository.InsertAsync(employeeId: 1, Monday, Monday.ToDateTime(new TimeOnly(9, 5)));

        Assert.That(result, Is.EqualTo(ClockInResult.AlreadyExists));
    }

    [Test]
    public async Task ClockInRepository_InsertAsync_OtherEmployeeSameDayIsInserted()
    {
        using var database = SqlServerTestDatabase.Create(Schema);
        var repository = new ClockInRepository(database.ConnectionString);
        await repository.InsertAsync(employeeId: 1, Monday, Monday.ToDateTime(new TimeOnly(9, 0)));

        var result = await repository.InsertAsync(employeeId: 2, Monday, Monday.ToDateTime(new TimeOnly(9, 0)));

        Assert.That(result, Is.EqualTo(ClockInResult.Inserted));
    }
}
