using System.Text;
using DotNet.Practices.DynamicMapping.Models;

namespace DotNet.Practices.DynamicMapping.Plans;

public readonly record struct PlanKey(string Value)
{
    public string Value { get; } = Value is { Length: 64 } && Value.All(Uri.IsHexDigit) && Value == Value.ToLowerInvariant()
        ? Value
        : throw new ArgumentException("A plan key is 64 lowercase hex characters.", nameof(Value));

    public static PlanKey For(PropertyBag bag, SourceSchema source)
    {
        ArgumentNullException.ThrowIfNull(bag);
        ArgumentNullException.ThrowIfNull(source);

        var canonical = new StringBuilder();
        canonical.Append("bag:").Append(bag.Id).Append('\n');
        canonical.Append("extends:").AppendJoin(',', bag.Extends.Order(StringComparer.Ordinal)).Append('\n');
        foreach (var property in bag.Properties.OrderBy(property => property.Id, StringComparer.Ordinal))
        {
            var validators = property.Validators;
            canonical.Append("  ")
                .Append(property.Id).Append('|')
                .Append(property.Type).Append('|')
                .Append(property.Lookup).Append('|')
                .Append(property.Relation).Append('|')
                .Append(property.DefaultValue).Append('|')
                .Append(property.ValueGenerator).Append('|')
                .Append(property.IdentifyingGenerator).Append('|')
                .Append(property.Unique).Append('|')
                .Append(property.ReadOnly).Append('|')
                .Append(validators.Required).Append('|')
                .Append(validators.Pattern).Append('|')
                .Append(validators.MaxLength).Append('|')
                .Append(validators.FutureDate).Append('\n');
        }

        canonical.Append("source:").Append(source.Name).Append('\n');
        foreach (var column in source.Columns.OrderBy(column => column.Name, StringComparer.Ordinal))
        {
            canonical.Append("  ").Append(column.Name).Append(':').Append(column.Type).Append('\n');
        }

        return new PlanKey(Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(canonical.ToString()))));
    }

    public override string ToString() => Value;
}
