using System.Globalization;
using System.Text.RegularExpressions;
using DotNet.Practices.DynamicMapping.Models;

namespace DotNet.Practices.DynamicMapping.Validators;

public static class PropertyValidator
{
    private static readonly string[] DateTimeFormats = ["yyyy-MM-dd'T'HH:mm:ssK", "yyyy-MM-dd'T'HH:mm:ss.FFFFFFFK"];

    // Patterns come from external JSON; a catastrophic one must fail the value, not stall the process.
    private static readonly TimeSpan PatternTimeout = TimeSpan.FromMilliseconds(250);

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
        if (validators.Pattern is not null && !Matches(value, validators.Pattern))
        {
            errors.Add("pattern");
        }

        if (validators.MaxLength is { } maxLength && value.Length > maxLength)
        {
            errors.Add($"maxlength {maxLength}");
        }

        if (validators.FutureDate && typeError is null && TryParseDate(value, out var date) && date <= DateOnly.FromDateTime(DateTime.UtcNow))
        {
            errors.Add("futureDate");
        }

        return errors;
    }

    private static bool Matches(string value, string pattern)
    {
        try
        {
            return Regex.IsMatch(value, pattern, RegexOptions.None, PatternTimeout);
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
    }

    private static string? TypeError(PropertyType? type, string value) => type switch
    {
        PropertyType.Date when !TryParseDate(value, out _) => "date",
        PropertyType.Boolean when !bool.TryParse(value, out _) => "boolean",
        PropertyType.Integer when !long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out _) => "integer",
        PropertyType.UniqueIdentifier when !Guid.TryParse(value, out _) => "uniqueIdentifier",
        _ => null,
    };

    // ISO date or ISO date-time only; DateTime.TryParse would accept "2030" as a time of day.
    private static bool TryParseDate(string value, out DateOnly date)
    {
        if (DateOnly.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
        {
            return true;
        }

        if (DateTimeOffset.TryParseExact(value, DateTimeFormats, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var moment))
        {
            date = DateOnly.FromDateTime(moment.UtcDateTime);
            return true;
        }

        date = default;
        return false;
    }
}
