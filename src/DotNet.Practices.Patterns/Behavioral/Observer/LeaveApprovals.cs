using DotNet.Practices.Patterns.Behavioral.Observer.Interfaces;

namespace DotNet.Practices.Patterns.Behavioral.Observer;

// Subject: knows its observers only through the interface, never who they are.
public sealed class LeaveApprovals
{
    private readonly List<IApprovalObserver> _observers = [];

    public void Subscribe(IApprovalObserver observer) => _observers.Add(observer);

    public void Unsubscribe(IApprovalObserver observer) => _observers.Remove(observer);

    public void Approve(LeaveRequest request)
    {
        foreach (var observer in _observers)
        {
            observer.OnApproved(request);
        }
    }
}
