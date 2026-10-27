using DotNet.Practices.Patterns.Behavioral.State.Abstracts;
using DotNet.Practices.Patterns.Behavioral.State.Interfaces;

namespace DotNet.Practices.Patterns.Behavioral.State.States;

public sealed class ApprovedState : LeaveState
{
    public override string Name => "Approved";

    public override ILeaveState Cancel() => new CancelledState();
}
