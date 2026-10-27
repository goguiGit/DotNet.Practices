using System.Data;

namespace DotNet.Practices.SqlErrors.SqlInjection;

public sealed class EmployeeDirectory(string connectionString)
{
    public async Task<IReadOnlyList<string>> FindByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);
        await using var command = new SqlCommand("SELECT Name FROM Employees WHERE Name = @name;", connection);
        command.Parameters.Add("@name", SqlDbType.NVarChar, 100).Value = name;

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        var names = new List<string>();
        while (await reader.ReadAsync(cancellationToken))
        {
            names.Add(reader.GetString(0));
        }

        return names;
    }
}
