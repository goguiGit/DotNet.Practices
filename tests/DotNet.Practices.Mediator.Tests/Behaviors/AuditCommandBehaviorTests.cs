using DotNet.Practices.Mediator.Behaviors.AuditTrail;
using DotNet.Practices.Mediator.Tests.Fixtures;

namespace DotNet.Practices.Mediator.Tests.Behaviors;

public class AuditCommandBehaviorTests
{
    [Test]
    public async Task Send_AuditableCommand_WritesEntryWithPayloadAndResponse()
    {
        var mediator = new TestMediator();

        await mediator.Mediator.Send(new RenamePetCommand(7, "Rex", "secret notes"));

        var entry = mediator.AuditSink.Entries.Single();
        Assert.Multiple(() =>
        {
            Assert.That(entry.CommandType, Is.EqualTo(typeof(RenamePetCommand).FullName));
            Assert.That(entry.FriendlyName, Is.EqualTo("Rename pet"));
            Assert.That(entry.Action, Is.EqualTo(AuditActionType.Update));
            Assert.That(entry.Succeeded, Is.True);
            Assert.That(entry.PayloadJson, Does.Contain("\"NewName\":\"Rex\""));
            Assert.That(entry.PayloadJson, Does.Not.Contain("secret notes"));
            Assert.That(entry.ResponseJson, Does.Contain("renamed to Rex"));
        });
    }

    [Test]
    public async Task Send_NonAuditableRequest_WritesNothing()
    {
        var mediator = new TestMediator();

        await mediator.Mediator.Send(new GetGreetingQuery("Ada"));

        Assert.That(mediator.AuditSink.Entries, Is.Empty);
    }

    [Test]
    public void Send_WhenHandlerThrows_WritesFailedEntryAndRethrows()
    {
        var mediator = new TestMediator();

        Assert.That(
            () => mediator.Mediator.Send(new RenamePetCommand(7, "", "n/a")),
            Throws.InvalidOperationException);

        var entry = mediator.AuditSink.Entries.Single();
        Assert.Multiple(() =>
        {
            Assert.That(entry.Succeeded, Is.False);
            Assert.That(entry.ResponseJson, Does.Contain(nameof(InvalidOperationException)));
        });
    }
}
