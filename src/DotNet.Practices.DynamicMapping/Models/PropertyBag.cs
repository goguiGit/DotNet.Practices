namespace DotNet.Practices.DynamicMapping.Models;

public record PropertyBag(string Id, IReadOnlyList<PropertyDefinition> Properties)
{
    // Ids of base bags whose properties this bag inherits and overrides. Resolved in a later step; may live in another file.
    public IReadOnlyList<string> Extends { get; init; } = [];
}
