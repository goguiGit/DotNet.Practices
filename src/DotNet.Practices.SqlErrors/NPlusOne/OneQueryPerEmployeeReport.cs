using System.Data;

namespace DotNet.Practices.SqlErrors.NPlusOne;

// The shape to avoid: one query for the parents and one more per parent. Kept so the tests can measure it.
public sealed class OneQueryPerEmployeeReport(string connectionString)
{
    public async Task<IReadOnlyList<EmployeeAbsences>> BuildAsync(CancellationToken cancellationToken = default)
    {
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        var employees = new List<(int Id, string Name)>();
        await using (var command = new SqlCommand("SELECT Id, Name FROM Employees ORDER BY Name;", connection))
        await using (var reader = await command.ExecuteReaderAsync(cancellationToken))
        {
            while (await reader.ReadAsync(cancellationToken))
            {
                employees.Add((reader.GetInt32(0), reader.GetString(1)));
            }
        }

        var rows = new List<EmployeeAbsences>();
        foreach (var (id, name) in employees)
        {
            rows.Add(new EmployeeAbsences(name, TeamAbsencesReport.Format(await AbsencesOfAsync(connection, id, cancellationToken))));
        }

        return rows;
    }

    private static async Task<List<DateOnly>> AbsencesOfAsync(SqlConnection connection, int employeeId, CancellationToken cancellationToken)
    {
        await using var command = new SqlCommand("SELECT Day FROM Absences WHERE EmployeeId = @employeeId ORDER BY Day;", connection);
        command.Parameters.Add("@employeeId", SqlDbType.Int).Value = employeeId;
        var days = new List<DateOnly>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            days.Add(reader.GetFieldValue<DateOnly>(0));
        }

        return days;
    }
}
