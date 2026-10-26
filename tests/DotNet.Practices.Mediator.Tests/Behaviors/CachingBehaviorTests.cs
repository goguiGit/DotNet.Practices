using DotNet.Practices.Mediator.Tests.Fixtures;

namespace DotNet.Practices.Mediator.Tests.Behaviors;

public class CachingBehaviorTests
{
    [Test]
    public async Task Send_SameCacheableQueryTwice_InvokesHandlerOnce()
    {
        var mediator = new TestMediator();

        var first = await mediator.Mediator.Send(new GetGreetingQuery("Ada"));
        var second = await mediator.Mediator.Send(new GetGreetingQuery("Ada"));

        Assert.Multiple(() =>
        {
            Assert.That(mediator.GreetingHandler.Calls, Is.EqualTo(1));
            Assert.That(second, Is.EqualTo(first));
        });
    }

    [Test]
    public async Task Send_QueriesWithDifferentCacheKeys_InvokesHandlerPerKey()
    {
        var mediator = new TestMediator();

        await mediator.Mediator.Send(new GetGreetingQuery("Ada"));
        await mediator.Mediator.Send(new GetGreetingQuery("Grace"));

        Assert.That(mediator.GreetingHandler.Calls, Is.EqualTo(2));
    }

    [Test]
    public async Task Send_NonCacheableRequestTwice_InvokesHandlerTwice()
    {
        var mediator = new TestMediator();

        await mediator.Mediator.Send(new RenamePetCommand(1, "Rex", "n/a"));
        await mediator.Mediator.Send(new RenamePetCommand(1, "Rex", "n/a"));

        Assert.That(mediator.RenameHandler.Calls, Is.EqualTo(2));
    }
}
