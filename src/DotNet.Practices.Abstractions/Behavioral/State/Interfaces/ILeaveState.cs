namespace DotNet.Practices.Abstractions.Behavioral.State.Interfaces;

// Each transition returns the next state; the request only swaps the reference.
public interface ILeaveState
{
    string Name { get; }

    ILeaveState Submit();

    ILeaveState Approve();

    ILeaveState Reject();

    ILeaveState Cancel();
}
