using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using DotNet.Practices.DynamicMapping.Models;

namespace DotNet.Practices.DynamicMapping.Plans;

public sealed record PlanKey(string Value)
{
    public string Value { get; } = Value is { Length: 64 } && Value.All(Uri.IsHexDigit) && Value == Value.ToLowerInvariant()
        ? Value
        : throw new ArgumentException("A plan key is 64 lowercase hex characters.", nameof(Value));

    // The canonical form is JSON so that nulls, empty strings and separators inside values can never collide.
    public static PlanKey For(PropertyBag bag, SourceSchema source, string compilerVersion)
    {
        ArgumentNullException.ThrowIfNull(bag);
        ArgumentNullException.ThrowIfNull(source);
        ArgumentException.ThrowIfNullOrWhiteSpace(compilerVersion);

        var canonical = new
        {
            CompilerVersion = compilerVersion,
            Bag = new
            {
                bag.Id,
                Extends = bag.Extends.Order(StringComparer.Ordinal),
                Properties = bag.Properties
                    .OrderBy(property => property.Id, StringComparer.Ordinal)
                    .Select(property => new
                    {
                        property.Id,
                        Type = property.Type?.ToString(),
                        property.Lookup,
                        property.Relation,
                        property.DefaultValue,
                        property.ValueGenerator,
                        property.IdentifyingGenerator,
                        property.Unique,
                        property.ReadOnly,
                        Validators = new
                        {
                            property.Validators.Required,
                            property.Validators.Pattern,
                            property.Validators.MaxLength,
                            property.Validators.FutureDate,
                        },
                    }),
            },
            Source = new
            {
                source.Name,
                Columns = source.Columns
                    .OrderBy(column => column.Name, StringComparer.Ordinal)
                    .Select(column => new { column.Name, column.Type }),
            },
        };

        var json = JsonSerializer.Serialize(canonical);
        return new PlanKey(Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(json))));
    }

    public override string ToString() => Value;
}
