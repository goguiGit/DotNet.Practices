using DotNet.Practices.Patterns.Behavioral.Observer;
using DotNet.Practices.Patterns.Behavioral.Observer.Observers;

namespace DotNet.Practices.Patterns.Tests.Behavioral.Observer;

public class LeaveApprovalsTests
{
    private static readonly LeaveRequest AnaMonday = new("Ana", new DateOnly(2026, 10, 5));

    [Test]
    public void LeaveApprovals_Approve_NotifiesThePayrollLedger()
    {
        var approvals = new LeaveApprovals();
        var ledger = new PayrollLedger();
        approvals.Subscribe(ledger);

        approvals.Approve(AnaMonday);

        Assert.That(ledger.Recorded, Is.EqualTo(new[] { AnaMonday }));
    }

    [Test]
    public void LeaveApprovals_Approve_NotifiesTheEmployeeMailbox()
    {
        var approvals = new LeaveApprovals();
        var mailbox = new EmployeeMailbox("Ana");
        approvals.Subscribe(mailbox);

        approvals.Approve(AnaMonday);

        Assert.That(mailbox.Inbox, Is.EqualTo(new[] { "Your leave on 2026-10-05 was approved" }));
    }

    [Test]
    public void EmployeeMailbox_OnApproved_IgnoresOtherPeoplesRequests()
    {
        var approvals = new LeaveApprovals();
        var mailbox = new EmployeeMailbox("Luis");
        approvals.Subscribe(mailbox);

        approvals.Approve(AnaMonday);

        Assert.That(mailbox.Inbox, Is.Empty);
    }

    [Test]
    public void LeaveApprovals_Unsubscribe_StopsNotifications()
    {
        var approvals = new LeaveApprovals();
        var ledger = new PayrollLedger();
        approvals.Subscribe(ledger);
        approvals.Unsubscribe(ledger);

        approvals.Approve(AnaMonday);

        Assert.That(ledger.Recorded, Is.Empty);
    }
}
