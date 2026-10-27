using DotNet.Practices.Abstractions.Behavioral.State.Abstracts;
using DotNet.Practices.Abstractions.Behavioral.State.Interfaces;

namespace DotNet.Practices.Abstractions.Behavioral.State.States;

public sealed class ApprovedState : LeaveState
{
    public override string Name => "Approved";

    public override ILeaveState Cancel() => new CancelledState();
}
