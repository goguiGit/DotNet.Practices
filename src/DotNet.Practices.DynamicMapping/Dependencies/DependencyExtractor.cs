using System.Text.RegularExpressions;

namespace DotNet.Practices.DynamicMapping.Dependencies;

// Only the first segment after Context counts: Context.Owner.Lastname is a dependency on Owner.
public static partial class DependencyExtractor
{
    public static IReadOnlySet<string> Extract(string? expression)
    {
        if (expression is null || !expression.TrimStart().StartsWith('$'))
        {
            return new HashSet<string>(StringComparer.Ordinal);
        }

        return ContextReference().Matches(expression)
            .Select(match => match.Groups["id"].Value)
            .ToHashSet(StringComparer.Ordinal);
    }

    [GeneratedRegex(@"(?<![\w.])Context\.(?<id>\w+)")]
    private static partial Regex ContextReference();
}
