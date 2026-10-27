using DotNet.Practices.Patterns.Behavioral.State.Interfaces;
using DotNet.Practices.Patterns.Behavioral.State.States;

namespace DotNet.Practices.Patterns.Behavioral.State;

// Context: no switch on status anywhere; the current state object decides.
public sealed class LeaveRequest
{
    private ILeaveState _state = new DraftState();

    public string Status => _state.Name;

    public void Submit() => _state = _state.Submit();

    public void Approve() => _state = _state.Approve();

    public void Reject() => _state = _state.Reject();

    public void Cancel() => _state = _state.Cancel();
}
