using DotNet.Practices.SqlErrors.SqlInjection;
using DotNet.Practices.SqlErrors.Tests.Infrastructure;
using Microsoft.Data.SqlClient;

namespace DotNet.Practices.SqlErrors.Tests.SqlInjection;

[Category("Integration")]
public class EmployeeDirectoryTests
{
    private const string Schema = """
        CREATE TABLE Employees (Id int PRIMARY KEY, Name nvarchar(100) NOT NULL);
        INSERT INTO Employees VALUES (1, N'Ana García'), (2, N'Luis Pérez'), (3, N'Siobhan O''Brien');
        """;

    private const string Injection = "' OR 1 = 1 --";

    [Test]
    public async Task EmployeeDirectory_FindByNameAsync_ReturnsTheEmployeeWithThatName()
    {
        using var database = SqlServerTestDatabase.Create(Schema);
        var directory = new EmployeeDirectory(database.ConnectionString);

        var names = await directory.FindByNameAsync("Ana García");

        Assert.That(names, Is.EqualTo(new[] { "Ana García" }));
    }

    [Test]
    public async Task EmployeeDirectory_FindByNameAsync_ApostropheIsPartOfTheName()
    {
        using var database = SqlServerTestDatabase.Create(Schema);
        var directory = new EmployeeDirectory(database.ConnectionString);

        var names = await directory.FindByNameAsync("Siobhan O'Brien");

        Assert.That(names, Is.EqualTo(new[] { "Siobhan O'Brien" }));
    }

    [Test]
    public async Task EmployeeDirectory_FindByNameAsync_InjectionIsJustANameNobodyHas()
    {
        using var database = SqlServerTestDatabase.Create(Schema);
        var directory = new EmployeeDirectory(database.ConnectionString);

        var names = await directory.FindByNameAsync(Injection);

        Assert.That(names, Is.Empty);
    }

    // The two tests below run the query the way it must never be written, to show what the parameters above prevent.
    [Test]
    public void ConcatenatedQuery_WithInjection_LeaksEveryEmployee()
    {
        using var database = SqlServerTestDatabase.Create(Schema);

        var names = ConcatenatedQuery(database, Injection);

        Assert.That(names, Has.Count.EqualTo(3));
    }

    [Test]
    public void ConcatenatedQuery_WithApostrophe_FailsWithIncorrectSyntax()
    {
        using var database = SqlServerTestDatabase.Create(Schema);

        var exception = Assert.Throws<SqlException>(() => ConcatenatedQuery(database, "Siobhan O'Brien"));

        Assert.That(exception.Number, Is.EqualTo(102));
    }

    private static List<string> ConcatenatedQuery(SqlServerTestDatabase database, string name)
    {
        using var connection = new SqlConnection(database.ConnectionString);
        connection.Open();
        using var command = new SqlCommand("SELECT Name FROM Employees WHERE Name = '" + name + "';", connection);
        using var reader = command.ExecuteReader();
        var names = new List<string>();
        while (reader.Read())
        {
            names.Add(reader.GetString(0));
        }

        return names;
    }
}
