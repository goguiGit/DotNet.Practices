using DotNet.Practices.Abstractions.Behavioral.State;

namespace DotNet.Practices.Abstractions.Tests.Behavioral.State;

public class LeaveRequestTests
{
    [Test]
    public void LeaveRequest_New_IsADraft()
    {
        Assert.That(new LeaveRequest().Status, Is.EqualTo("Draft"));
    }

    [Test]
    public void LeaveRequest_SubmitThenApprove_IsApproved()
    {
        var request = new LeaveRequest();

        request.Submit();
        request.Approve();

        Assert.That(request.Status, Is.EqualTo("Approved"));
    }

    [Test]
    public void LeaveRequest_SubmitThenReject_IsRejected()
    {
        var request = new LeaveRequest();

        request.Submit();
        request.Reject();

        Assert.That(request.Status, Is.EqualTo("Rejected"));
    }

    [Test]
    public void LeaveRequest_CancelWhenApproved_IsCancelled()
    {
        var request = new LeaveRequest();
        request.Submit();
        request.Approve();

        request.Cancel();

        Assert.That(request.Status, Is.EqualTo("Cancelled"));
    }

    [Test]
    public void LeaveRequest_ApproveFromDraft_Throws()
    {
        var request = new LeaveRequest();

        Assert.That(() => request.Approve(), Throws.InvalidOperationException);
    }

    [Test]
    public void LeaveRequest_SubmitWhenRejected_Throws()
    {
        var request = new LeaveRequest();
        request.Submit();
        request.Reject();

        Assert.That(() => request.Submit(), Throws.InvalidOperationException);
    }
}
