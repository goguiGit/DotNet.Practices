namespace DotNet.Practices.DynamicMapping.Models;

public sealed record PropertyBag(string Id, IReadOnlyList<PropertyDefinition> Properties)
{
    public string Id { get; } = !string.IsNullOrWhiteSpace(Id)
        ? Id
        : throw new ArgumentException("Bag id is required.", nameof(Id));

    public IReadOnlyList<PropertyDefinition> Properties { get; } = WithoutDuplicates(Properties);

    // Resolved in a later step; the base bag may live in another file.
    public IReadOnlyList<string> Extends { get; init; } = [];

    private static IReadOnlyList<PropertyDefinition> WithoutDuplicates(IReadOnlyList<PropertyDefinition> properties)
    {
        ArgumentNullException.ThrowIfNull(properties);

        var duplicate = properties.GroupBy(property => property.Id, StringComparer.Ordinal).FirstOrDefault(group => group.Count() > 1);
        return duplicate is null
            ? properties
            : throw new ArgumentException($"Property '{duplicate.Key}' is declared more than once.", nameof(properties));
    }
}
