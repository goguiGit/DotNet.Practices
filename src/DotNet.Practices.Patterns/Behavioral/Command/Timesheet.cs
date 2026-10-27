namespace DotNet.Practices.Patterns.Behavioral.Command;

// Receiver: knows how to change itself, knows nothing about commands or history.
public sealed class Timesheet
{
    private readonly List<ClockEntry> _entries = [];

    public IReadOnlyList<ClockEntry> Entries => _entries;

    public void Add(ClockEntry entry) => _entries.Add(entry);

    public void Remove(ClockEntry entry) => _entries.Remove(entry);

    public void Replace(ClockEntry existing, ClockEntry replacement) => _entries[_entries.IndexOf(existing)] = replacement;
}
