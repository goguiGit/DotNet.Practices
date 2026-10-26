using System.Dynamic;
using System.Globalization;
using System.Reflection;

namespace DotNet.Practices.ScriptedMapping.Services;

// The `Services.*` catalogue scripts call. Dynamic dispatch by method name, as in Rabbit's ScriptGlobalsServices.
public sealed class ScriptServices(ISequenceTracker sequences) : DynamicObject
{
    private const string Base36Digits = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    public string GetFiveTwoOneIdValue(string first, string second, bool upper, string suffix = "")
    {
        var firstPart = Normalize(first, upper, nameof(first));
        var secondPart = Normalize(second, upper, nameof(second));
        var firstFive = firstPart.Length > 5 ? firstPart[..5] : firstPart;
        var secondTwo = secondPart.Length > 2 ? secondPart[..2] : secondPart;

        var counter = ToBase36(sequences.Next($"UniquePropertyBagPropertyId.{firstFive}{secondPart[0]}{suffix}"));
        if (counter.Length > 2)
        {
            throw new ArgumentOutOfRangeException(nameof(first), "The 5-2-1 sequence is exhausted for this name.");
        }

        // The id keeps eight characters before the suffix: a two-digit counter steals one from the second part.
        var secondComponent = counter.Length == 1 ? secondTwo : secondTwo[..1];
        return $"{firstFive}{secondComponent}{counter}{suffix}";
    }

    public string GetProvisioningContainer(IDictionary<string, string> mapping, string key) =>
        mapping.TryGetValue(key, out var container) ? container : string.Empty;

    public string GetIdToCheck(string domain, string user) =>
        string.IsNullOrEmpty(domain) || string.IsNullOrEmpty(user) ? string.Empty : $@"{domain}\{user}";

    public string GetNextIdValue(string template, int initial)
    {
        if (!template.Contains("{0", StringComparison.Ordinal))
        {
            throw new ArgumentException("The template must contain a {0} placeholder.", nameof(template));
        }

        return string.Format(CultureInfo.InvariantCulture, template, sequences.Next($"UniquePropertyBagPropertyId.{template}", initial));
    }

    public string Combine(string first, string second) => Path.Combine(first, second);

    public override bool TryInvokeMember(InvokeMemberBinder binder, object?[]? args, out object? result)
    {
        var method = GetType().GetMethod(binder.Name, BindingFlags.Public | BindingFlags.Instance)
            ?? throw new MissingMethodException(nameof(ScriptServices), binder.Name);

        var parameters = method.GetParameters();
        var arguments = new object?[parameters.Length];
        for (var i = 0; i < parameters.Length; i++)
        {
            arguments[i] = args is not null && i < args.Length ? args[i] : Type.Missing;
        }

        result = method.Invoke(this, BindingFlags.OptionalParamBinding, null, arguments, CultureInfo.InvariantCulture);
        return true;
    }

    private static string Normalize(string? value, bool upper, string parameterName)
    {
        var compact = value?.Replace(" ", string.Empty) ?? string.Empty;
        if (compact.Length == 0)
        {
            throw new ArgumentException("Name parts cannot be empty.", parameterName);
        }

        return upper ? compact.ToUpperInvariant() : compact.ToLowerInvariant();
    }

    private static string ToBase36(int value)
    {
        var digits = string.Empty;
        do
        {
            digits = Base36Digits[value % 36] + digits;
            value /= 36;
        }
        while (value > 0);

        return digits;
    }
}
