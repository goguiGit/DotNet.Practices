using DotNet.Practices.Patterns.Behavioral.State.Abstracts;

namespace DotNet.Practices.Patterns.Behavioral.State.States;

public sealed class CancelledState : LeaveState
{
    public override string Name => "Cancelled";
}
