namespace DotNet.Practices.DynamicMapping.Models;

public record SourceColumn(string Name, string Type);

public record SourceSchema(string Name, IReadOnlyList<SourceColumn> Columns);
