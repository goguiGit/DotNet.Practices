using System.Globalization;
using System.Text.RegularExpressions;
using DotNet.Practices.DynamicMapping.Models;

namespace DotNet.Practices.DynamicMapping.Validators;

public static class PropertyValidator
{
    public static IReadOnlyList<string> Validate(PropertyDefinition property, string? value)
    {
        ArgumentNullException.ThrowIfNull(property);

        if (string.IsNullOrWhiteSpace(value))
        {
            return property.Validators.Required ? ["required"] : [];
        }

        var errors = new List<string>();
        var typeError = TypeError(property.Type, value);
        if (typeError is not null)
        {
            errors.Add(typeError);
        }

        var validators = property.Validators;
        if (validators.Pattern is not null && !Regex.IsMatch(value, validators.Pattern))
        {
            errors.Add("pattern");
        }

        if (validators.MaxLength is { } maxLength && value.Length > maxLength)
        {
            errors.Add($"maxlength {maxLength}");
        }

        if (validators.FutureDate && typeError is null && TryParseDate(value, out var date) && date <= DateTime.UtcNow.Date)
        {
            errors.Add("futureDate");
        }

        return errors;
    }

    private static string? TypeError(PropertyType? type, string value) => type switch
    {
        PropertyType.Date when !TryParseDate(value, out _) => "date",
        PropertyType.Boolean when !bool.TryParse(value, out _) => "boolean",
        PropertyType.Integer when !long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out _) => "integer",
        PropertyType.UniqueIdentifier when !Guid.TryParse(value, out _) => "uniqueIdentifier",
        _ => null,
    };

    private static bool TryParseDate(string value, out DateTime date) =>
        DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out date);
}
