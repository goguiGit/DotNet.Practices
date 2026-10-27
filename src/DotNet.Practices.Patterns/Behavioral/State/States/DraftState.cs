using DotNet.Practices.Patterns.Behavioral.State.Abstracts;
using DotNet.Practices.Patterns.Behavioral.State.Interfaces;

namespace DotNet.Practices.Patterns.Behavioral.State.States;

public sealed class DraftState : LeaveState
{
    public override string Name => "Draft";

    public override ILeaveState Submit() => new SubmittedState();

    public override ILeaveState Cancel() => new CancelledState();
}
