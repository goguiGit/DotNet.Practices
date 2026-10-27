using DotNet.Practices.Abstractions.Behavioral.State.Abstracts;

namespace DotNet.Practices.Abstractions.Behavioral.State.States;

public sealed class RejectedState : LeaveState
{
    public override string Name => "Rejected";
}
