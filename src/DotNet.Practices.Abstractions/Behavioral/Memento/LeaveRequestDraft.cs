using DotNet.Practices.Abstractions.Behavioral.Memento.Interfaces;

namespace DotNet.Practices.Abstractions.Behavioral.Memento;

// Originator: the only type that can create a snapshot and read it back.
public sealed class LeaveRequestDraft
{
    private readonly List<DateOnly> _days = [];

    public IReadOnlyList<DateOnly> Days => _days;

    public string? Reason { get; set; }

    public void AddDay(DateOnly day) => _days.Add(day);

    public IDraftSnapshot Save() => new Snapshot([.. _days], Reason);

    public void Restore(IDraftSnapshot snapshot)
    {
        var state = (Snapshot)snapshot;
        _days.Clear();
        _days.AddRange(state.Days);
        Reason = state.Reason;
    }

    private sealed record Snapshot(IReadOnlyList<DateOnly> Days, string? Reason) : IDraftSnapshot;
}
