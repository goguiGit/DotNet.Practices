using DotNet.Practices.Patterns.Behavioral.State.Abstracts;
using DotNet.Practices.Patterns.Behavioral.State.Interfaces;

namespace DotNet.Practices.Patterns.Behavioral.State.States;

public sealed class SubmittedState : LeaveState
{
    public override string Name => "Submitted";

    public override ILeaveState Approve() => new ApprovedState();

    public override ILeaveState Reject() => new RejectedState();

    public override ILeaveState Cancel() => new CancelledState();
}
