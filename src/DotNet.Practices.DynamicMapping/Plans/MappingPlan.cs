namespace DotNet.Practices.DynamicMapping.Plans;

public sealed record MappingPlan(PlanKey Key, string CompilerVersion, IReadOnlyList<PropertyMapping> Properties)
{
    public IReadOnlyList<PropertyMapping> Properties { get; } = WithoutDuplicates(Properties);

    public bool HasDynamicProperties => Properties.Any(property => property.Kind == MappingKind.Dynamic);

    private static IReadOnlyList<PropertyMapping> WithoutDuplicates(IReadOnlyList<PropertyMapping> properties)
    {
        ArgumentNullException.ThrowIfNull(properties);

        var duplicate = properties.GroupBy(property => property.PropertyId).FirstOrDefault(group => group.Count() > 1);
        return duplicate is null
            ? properties
            : throw new ArgumentException($"Property '{duplicate.Key}' is mapped more than once.", nameof(properties));
    }
}
