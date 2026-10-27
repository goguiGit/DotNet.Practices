using DotNet.Practices.Patterns.Behavioral.Observer.Interfaces;

namespace DotNet.Practices.Patterns.Behavioral.Observer.Observers;

public sealed class PayrollLedger : IApprovalObserver
{
    private readonly List<LeaveRequest> _recorded = [];

    public IReadOnlyList<LeaveRequest> Recorded => _recorded;

    public void OnApproved(LeaveRequest request) => _recorded.Add(request);
}
