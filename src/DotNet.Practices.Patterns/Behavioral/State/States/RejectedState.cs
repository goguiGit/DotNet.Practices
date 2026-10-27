using DotNet.Practices.Patterns.Behavioral.State.Abstracts;

namespace DotNet.Practices.Patterns.Behavioral.State.States;

public sealed class RejectedState : LeaveState
{
    public override string Name => "Rejected";
}
