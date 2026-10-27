using DotNet.Practices.Abstractions.Behavioral.Memento;

namespace DotNet.Practices.Abstractions.Tests.Behavioral.Memento;

public class LeaveRequestDraftTests
{
    private static readonly DateOnly Monday = new(2026, 10, 5);
    private static readonly DateOnly Tuesday = new(2026, 10, 6);

    [Test]
    public void LeaveRequestDraft_Restore_BringsBackTheStateAtTheSnapshot()
    {
        var draft = new LeaveRequestDraft();
        draft.AddDay(Monday);
        draft.Reason = "Family";
        var snapshot = draft.Save();
        draft.AddDay(Tuesday);
        draft.Reason = "Trip";

        draft.Restore(snapshot);

        Assert.That(draft, Has.Property("Reason").EqualTo("Family").And.Property("Days").EqualTo(new[] { Monday }));
    }

    [Test]
    public void DraftHistory_Undo_RestoresThePreviousSnapshot()
    {
        var draft = new LeaveRequestDraft();
        var history = new DraftHistory();
        history.Save(draft);
        draft.AddDay(Monday);
        history.Save(draft);
        draft.AddDay(Tuesday);

        history.Undo(draft);

        Assert.That(draft.Days, Is.EqualTo(new[] { Monday }));
    }

    [Test]
    public void DraftHistory_Undo_Twice_WalksBackTwoSnapshots()
    {
        var draft = new LeaveRequestDraft();
        var history = new DraftHistory();
        history.Save(draft);
        draft.AddDay(Monday);
        history.Save(draft);
        draft.AddDay(Tuesday);

        history.Undo(draft);
        history.Undo(draft);

        Assert.That(draft.Days, Is.Empty);
    }

    [Test]
    public void DraftHistory_Undo_WithNothingSaved_LeavesTheDraftUntouched()
    {
        var draft = new LeaveRequestDraft();
        draft.AddDay(Monday);

        new DraftHistory().Undo(draft);

        Assert.That(draft.Days, Is.EqualTo(new[] { Monday }));
    }
}
