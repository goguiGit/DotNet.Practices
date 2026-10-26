using DotNet.Practices.DynamicMapping.Validators;

namespace DotNet.Practices.DynamicMapping.Models;

// Display names and error messages are left out on purpose: they must not change the plan key.
// Type is null only when the bag extends another and this property overrides one of the base.
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
