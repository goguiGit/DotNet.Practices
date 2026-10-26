namespace DotNet.Practices.ScriptedMapping.Services;

// Rabbit backs this with a database table and a global reservation; here a dictionary is enough to show the mechanics.
public sealed class InMemorySequenceTracker : ISequenceTracker
{
    private readonly Dictionary<string, int> _next = new(StringComparer.Ordinal);

    public int Next(string sequence, int initial = 1)
    {
        var value = _next.TryGetValue(sequence, out var current) ? current : initial;
        _next[sequence] = value + 1;
        return value;
    }
}
