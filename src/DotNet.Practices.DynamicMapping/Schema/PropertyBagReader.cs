using System.Text.Json;
using DotNet.Practices.DynamicMapping.Models;
using DotNet.Practices.DynamicMapping.Validators;

namespace DotNet.Practices.DynamicMapping.Schema;

public static class PropertyBagReader
{
    private static readonly JsonDocumentOptions DocumentOptions = new()
    {
        AllowTrailingCommas = true,
        CommentHandling = JsonCommentHandling.Skip,
    };

    private static readonly Dictionary<string, PropertyType> TypeNames = new(StringComparer.OrdinalIgnoreCase)
    {
        ["text"] = PropertyType.Text,
        ["lookup"] = PropertyType.Lookup,
        ["date"] = PropertyType.Date,
        ["boolean"] = PropertyType.Boolean,
        ["integer"] = PropertyType.Integer,
        ["uniqueIdentifier"] = PropertyType.UniqueIdentifier,
        ["relation"] = PropertyType.Relation,
    };

    public static IReadOnlyList<PropertyBag> ReadFile(string path) => Read(File.ReadAllText(path));

    public static IReadOnlyList<PropertyBag> Read(string json)
    {
        ArgumentNullException.ThrowIfNull(json);

        using var document = JsonDocument.Parse(json, DocumentOptions);
        if (document.RootElement.ValueKind != JsonValueKind.Array)
        {
            throw new FormatException("A property bags file is a JSON array of bags.");
        }

        return document.RootElement.EnumerateArray().Select(ReadBag).ToList();
    }

    private static PropertyBag ReadBag(JsonElement element)
    {
        var id = RequiredString(element, "id", "bag");
        var properties = TryGet(element, "properties", out var propertiesElement)
            ? propertiesElement.EnumerateArray().Select(property => ReadProperty(property, id)).ToList()
            : [];
        var extends = TryGet(element, "extends", out var extendsElement)
            ? extendsElement.EnumerateArray().Select(baseId => baseId.GetString()!).ToList()
            : [];

        return new PropertyBag(id, properties) { Extends = extends };
    }

    private static PropertyDefinition ReadProperty(JsonElement element, string bagId)
    {
        var id = RequiredString(element, "id", $"property of bag '{bagId}'");

        return new PropertyDefinition(id, ReadType(element, id))
        {
            Lookup = OptionalText(element, "lookup"),
            Relation = OptionalText(element, "relation"),
            DefaultValue = OptionalText(element, "defaultValue"),
            ValueGenerator = OptionalText(element, "valueGenerator"),
            IdentifyingGenerator = OptionalText(element, "identifyingGenerator"),
            Unique = OptionalText(element, "unique"),
            ReadOnly = OptionalBool(element, "readOnly"),
            Validators = ReadValidators(element),
        };
    }

    private static PropertyType? ReadType(JsonElement element, string propertyId)
    {
        var typeName = OptionalText(element, "type");
        if (typeName is null)
        {
            return null;
        }

        return TypeNames.TryGetValue(typeName, out var type)
            ? type
            : throw new FormatException($"Property '{propertyId}' has unknown type '{typeName}'.");
    }

    private static PropertyValidators ReadValidators(JsonElement element)
    {
        if (!TryGet(element, "validators", out var validators) || validators.ValueKind != JsonValueKind.Object)
        {
            return PropertyValidators.None;
        }

        return new PropertyValidators
        {
            Required = OptionalBool(validators, "required"),
            Pattern = OptionalText(validators, "pattern"),
            MaxLength = TryGet(validators, "maxlength", out var maxLength) && maxLength.TryGetInt32(out var length) ? length : null,
            FutureDate = OptionalBool(validators, "futureDate"),
        };
    }

    private static string RequiredString(JsonElement element, string name, string owner) =>
        OptionalText(element, name) ?? throw new FormatException($"Missing '{name}' on {owner}.");

    // Expressions are strings, but literal defaults can be booleans or numbers; keep their JSON text.
    private static string? OptionalText(JsonElement element, string name) =>
        TryGet(element, name, out var value) switch
        {
            false => null,
            true when value.ValueKind == JsonValueKind.String => value.GetString(),
            true when value.ValueKind is JsonValueKind.Null or JsonValueKind.Object or JsonValueKind.Array => null,
            true => value.GetRawText(),
        };

    private static bool OptionalBool(JsonElement element, string name) =>
        TryGet(element, name, out var value) && value.ValueKind == JsonValueKind.True;

    // Property names in the real files are not consistently cased (`readonly` vs `readOnly`).
    private static bool TryGet(JsonElement element, string name, out JsonElement value)
    {
        foreach (var property in element.EnumerateObject())
        {
            if (property.NameEquals(name) || string.Equals(property.Name, name, StringComparison.OrdinalIgnoreCase))
            {
                value = property.Value;
                return true;
            }
        }

        value = default;
        return false;
    }
}
