using DotNet.Practices.Abstractions.Behavioral.Mediator.Colleagues;
using DotNet.Practices.Abstractions.Behavioral.Mediator.Interfaces;

namespace DotNet.Practices.Abstractions.Behavioral.Mediator;

// Mediator: the only place that knows the whole leave workflow and every colleague in it.
public sealed class LeaveDesk : ILeaveDesk
{
    private readonly Dictionary<LeaveRequest, Employee> _requesters = [];

    public LeaveDesk(int capacityPerDay)
    {
        Manager = new Manager(this);
        Calendar = new TeamCalendar(capacityPerDay);
    }

    public Manager Manager { get; }

    public TeamCalendar Calendar { get; }

    public void Request(Employee from, DateOnly day)
    {
        var request = new LeaveRequest(from.Name, day);
        if (Calendar.IsFull(day))
        {
            from.Receive($"Leave on {day:yyyy-MM-dd} rejected: team calendar full");
            return;
        }

        _requesters[request] = from;
        Manager.Review(request);
    }

    public void Approve(LeaveRequest request)
    {
        Calendar.Block(request.Day, request.Employee);
        _requesters[request].Receive($"Leave on {request.Day:yyyy-MM-dd} approved");
    }
}
