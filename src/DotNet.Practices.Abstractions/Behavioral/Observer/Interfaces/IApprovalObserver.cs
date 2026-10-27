namespace DotNet.Practices.Abstractions.Behavioral.Observer.Interfaces;

public interface IApprovalObserver
{
    void OnApproved(LeaveRequest request);
}
