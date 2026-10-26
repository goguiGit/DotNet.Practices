namespace DotNet.Practices.DynamicMapping.Plans;

public record PropertyMapping(string PropertyId, MappingKind Kind, string? Expression = null, IReadOnlyList<string>? DependsOn = null)
{
    public string PropertyId { get; } = !string.IsNullOrWhiteSpace(PropertyId)
        ? PropertyId
        : throw new ArgumentException("Property id is required.", nameof(PropertyId));

    public string? Expression { get; } = Kind != MappingKind.Expression || !string.IsNullOrWhiteSpace(Expression)
        ? Expression
        : throw new ArgumentException($"Mapping '{PropertyId}' is an expression but has none.", nameof(Expression));

    public IReadOnlyList<string> DependsOn { get; } = DependsOn ?? [];
}
