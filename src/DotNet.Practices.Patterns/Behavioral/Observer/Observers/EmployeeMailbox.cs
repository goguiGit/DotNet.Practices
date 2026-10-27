using DotNet.Practices.Patterns.Behavioral.Observer.Interfaces;

namespace DotNet.Practices.Patterns.Behavioral.Observer.Observers;

public sealed class EmployeeMailbox(string employee) : IApprovalObserver
{
    private readonly List<string> _inbox = [];

    public IReadOnlyList<string> Inbox => _inbox;

    public void OnApproved(LeaveRequest request)
    {
        if (request.Employee == employee)
        {
            _inbox.Add($"Your leave on {request.Day:yyyy-MM-dd} was approved");
        }
    }
}
