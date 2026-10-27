using DotNet.Practices.Patterns.Behavioral.Memento.Interfaces;

namespace DotNet.Practices.Patterns.Behavioral.Memento;

// Caretaker: stores snapshots in order without knowing what is in them.
public sealed class DraftHistory
{
    private readonly Stack<IDraftSnapshot> _snapshots = new();

    public void Save(LeaveRequestDraft draft) => _snapshots.Push(draft.Save());

    public void Undo(LeaveRequestDraft draft)
    {
        if (_snapshots.TryPop(out var snapshot))
        {
            draft.Restore(snapshot);
        }
    }
}
