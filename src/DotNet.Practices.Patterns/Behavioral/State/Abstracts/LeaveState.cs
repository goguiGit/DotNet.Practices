using DotNet.Practices.Patterns.Behavioral.State.Interfaces;

namespace DotNet.Practices.Patterns.Behavioral.State.Abstracts;

// Every transition is forbidden unless a concrete state overrides it.
public abstract class LeaveState : ILeaveState
{
    public abstract string Name { get; }

    public virtual ILeaveState Submit() => throw Forbidden(nameof(Submit));

    public virtual ILeaveState Approve() => throw Forbidden(nameof(Approve));

    public virtual ILeaveState Reject() => throw Forbidden(nameof(Reject));

    public virtual ILeaveState Cancel() => throw Forbidden(nameof(Cancel));

    private InvalidOperationException Forbidden(string transition) =>
        new($"Cannot {transition.ToLowerInvariant()} a leave request that is {Name.ToLowerInvariant()}.");
}
