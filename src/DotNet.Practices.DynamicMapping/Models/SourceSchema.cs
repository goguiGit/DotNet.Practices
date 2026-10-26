namespace DotNet.Practices.DynamicMapping.Models;

public sealed record SourceColumn(string Name, string Type);

public sealed record SourceSchema(string Name, IReadOnlyList<SourceColumn> Columns);
