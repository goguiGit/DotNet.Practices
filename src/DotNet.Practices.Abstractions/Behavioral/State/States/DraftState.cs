using DotNet.Practices.Abstractions.Behavioral.State.Abstracts;
using DotNet.Practices.Abstractions.Behavioral.State.Interfaces;

namespace DotNet.Practices.Abstractions.Behavioral.State.States;

public sealed class DraftState : LeaveState
{
    public override string Name => "Draft";

    public override ILeaveState Submit() => new SubmittedState();

    public override ILeaveState Cancel() => new CancelledState();
}
