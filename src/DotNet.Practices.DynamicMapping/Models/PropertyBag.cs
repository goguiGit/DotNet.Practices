namespace DotNet.Practices.DynamicMapping.Models;

public record PropertyBag(string Id, IReadOnlyList<PropertyDefinition> Properties)
{
    // Resolved in a later step; the base bag may live in another file.
    public IReadOnlyList<string> Extends { get; init; } = [];
}
