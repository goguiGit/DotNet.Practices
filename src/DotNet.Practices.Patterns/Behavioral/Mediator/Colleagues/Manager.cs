using DotNet.Practices.Patterns.Behavioral.Mediator.Interfaces;

namespace DotNet.Practices.Patterns.Behavioral.Mediator.Colleagues;

public sealed class Manager(ILeaveDesk desk)
{
    private readonly List<LeaveRequest> _pending = [];

    public IReadOnlyList<LeaveRequest> Pending => _pending;

    public void Review(LeaveRequest request) => _pending.Add(request);

    public void Approve(LeaveRequest request)
    {
        _pending.Remove(request);
        desk.Approve(request);
    }
}
