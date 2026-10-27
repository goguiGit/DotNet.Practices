using DotNet.Practices.Patterns.Behavioral.Mediator;
using DotNet.Practices.Patterns.Behavioral.Mediator.Colleagues;

namespace DotNet.Practices.Patterns.Tests.Behavioral.Mediator;

public class LeaveDeskTests
{
    private static readonly DateOnly Monday = new(2026, 10, 5);

    [Test]
    public void Employee_RequestLeave_ReachesTheManagerThroughTheDesk()
    {
        var desk = new LeaveDesk(capacityPerDay: 1);
        var ana = new Employee("Ana", desk);

        ana.RequestLeave(Monday);

        Assert.That(desk.Manager.Pending, Is.EqualTo(new[] { new LeaveRequest("Ana", Monday) }));
    }

    [Test]
    public void Manager_Approve_NotifiesTheEmployee()
    {
        var desk = new LeaveDesk(capacityPerDay: 1);
        var ana = new Employee("Ana", desk);
        ana.RequestLeave(Monday);

        desk.Manager.Approve(new LeaveRequest("Ana", Monday));

        Assert.That(ana.Inbox, Is.EqualTo(new[] { "Leave on 2026-10-05 approved" }));
    }

    [Test]
    public void Manager_Approve_BlocksTheDayInTheTeamCalendar()
    {
        var desk = new LeaveDesk(capacityPerDay: 1);
        var ana = new Employee("Ana", desk);
        ana.RequestLeave(Monday);

        desk.Manager.Approve(new LeaveRequest("Ana", Monday));

        Assert.That(desk.Calendar.WhoIsOff(Monday), Is.EqualTo(new[] { "Ana" }));
    }

    [Test]
    public void Employee_RequestLeave_WhenTheTeamCalendarIsFull_IsRejectedWithoutReachingTheManager()
    {
        var desk = new LeaveDesk(capacityPerDay: 1);
        var ana = new Employee("Ana", desk);
        var luis = new Employee("Luis", desk);
        ana.RequestLeave(Monday);
        desk.Manager.Approve(new LeaveRequest("Ana", Monday));

        luis.RequestLeave(Monday);

        Assert.That(luis.Inbox, Is.EqualTo(new[] { "Leave on 2026-10-05 rejected: team calendar full" }));
    }

    [Test]
    public void Employee_RequestLeave_WhenRejected_LeavesNothingPendingForTheManager()
    {
        var desk = new LeaveDesk(capacityPerDay: 0);
        var ana = new Employee("Ana", desk);

        ana.RequestLeave(Monday);

        Assert.That(desk.Manager.Pending, Is.Empty);
    }
}
