namespace DotNet.Practices.DynamicMapping.Plans;

public sealed record PropertyMapping(string PropertyId, MappingKind Kind, string? Expression = null, IReadOnlyList<string>? DependsOn = null)
{
    public string PropertyId { get; } = !string.IsNullOrWhiteSpace(PropertyId)
        ? PropertyId
        : throw new ArgumentException("Property id is required.", nameof(PropertyId));

    // Kind and Expression are get-only on purpose: `with` copies fields without re-running validation.
    public MappingKind Kind { get; } = Kind;

    public string? Expression { get; } = Kind != MappingKind.Expression || !string.IsNullOrWhiteSpace(Expression)
        ? Expression
        : throw new ArgumentException($"Mapping '{PropertyId}' is an expression but has none.", nameof(Expression));

    public IReadOnlyList<string> DependsOn { get; init; } = DependsOn ?? [];
}
