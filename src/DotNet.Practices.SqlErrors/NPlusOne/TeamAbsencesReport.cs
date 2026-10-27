namespace DotNet.Practices.SqlErrors.NPlusOne;

public sealed class TeamAbsencesReport(string connectionString)
{
    public async Task<IReadOnlyList<EmployeeAbsences>> BuildAsync(CancellationToken cancellationToken = default)
    {
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);
        await using var command = new SqlCommand(
            """
            SELECT e.Name, a.Day
            FROM Employees e
            LEFT JOIN Absences a ON a.EmployeeId = e.Id
            ORDER BY e.Name, a.Day;
            """,
            connection);

        var days = new Dictionary<string, List<DateOnly>>();
        var order = new List<string>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            var name = reader.GetString(0);
            if (days.TryAdd(name, []))
            {
                order.Add(name);
            }

            if (!await reader.IsDBNullAsync(1, cancellationToken))
            {
                days[name].Add(reader.GetFieldValue<DateOnly>(1));
            }
        }

        return order.Select(name => new EmployeeAbsences(name, Format(days[name]))).ToList();
    }

    internal static string Format(IEnumerable<DateOnly> days) =>
        string.Join(", ", days.Select(day => day.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture)));
}
