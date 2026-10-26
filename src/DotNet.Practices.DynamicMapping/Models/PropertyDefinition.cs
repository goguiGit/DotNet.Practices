using DotNet.Practices.DynamicMapping.Validators;

namespace DotNet.Practices.DynamicMapping.Models;

// Only what affects how a value is produced. Display names and error messages are deliberately absent:
// they must not change the mapping plan key.
// Type is null only for properties of a bag that extends another: they override a base property and inherit its type.
public record PropertyDefinition(string Id, PropertyType? Type)
{
    public string? Lookup { get; init; }

    public string? Relation { get; init; }

    public string? DefaultValue { get; init; }

    public string? ValueGenerator { get; init; }

    public string? IdentifyingGenerator { get; init; }

    public string? Unique { get; init; }

    public bool ReadOnly { get; init; }

    public PropertyValidators Validators { get; init; } = PropertyValidators.None;
}
