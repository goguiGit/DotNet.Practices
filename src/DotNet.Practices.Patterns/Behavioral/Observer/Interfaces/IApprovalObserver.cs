namespace DotNet.Practices.Patterns.Behavioral.Observer.Interfaces;

public interface IApprovalObserver
{
    void OnApproved(LeaveRequest request);
}
