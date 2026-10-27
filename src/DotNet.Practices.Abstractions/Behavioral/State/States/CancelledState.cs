using DotNet.Practices.Abstractions.Behavioral.State.Abstracts;

namespace DotNet.Practices.Abstractions.Behavioral.State.States;

public sealed class CancelledState : LeaveState
{
    public override string Name => "Cancelled";
}
