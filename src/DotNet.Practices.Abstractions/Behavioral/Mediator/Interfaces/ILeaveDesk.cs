using DotNet.Practices.Abstractions.Behavioral.Mediator.Colleagues;

namespace DotNet.Practices.Abstractions.Behavioral.Mediator.Interfaces;

// Colleagues only know this; who else is involved is the desk's business.
public interface ILeaveDesk
{
    void Request(Employee from, DateOnly day);

    void Approve(LeaveRequest request);
}
