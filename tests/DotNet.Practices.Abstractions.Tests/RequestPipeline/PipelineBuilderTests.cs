using System.Net;
using DotNet.Practices.Abstractions.RequestPipeline;
using DotNet.Practices.Abstractions.RequestPipeline.Middlewares;

namespace DotNet.Practices.Abstractions.Tests.RequestPipeline;

public class PipelineBuilderTests
{
    private static Task Ok(RequestContext context)
    {
        context.StatusCode = HttpStatusCode.OK;
        context.Trace.Add("terminal");
        return Task.CompletedTask;
    }

    [Test]
    public async Task Build_WithNoMiddleware_InvokesTerminalDirectly()
    {
        var pipeline = new PipelineBuilder().Build(Ok);
        var context = new RequestContext("/home", user: "alice");

        await pipeline(context);

        Assert.That(context.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task Build_RunsMiddlewaresInRegistrationOrderAroundTerminal()
    {
        var pipeline = new PipelineBuilder()
            .Use(new LoggingMiddleware())
            .Use(new AuthenticationMiddleware())
            .Build(Ok);
        var context = new RequestContext("/home", user: "alice");

        await pipeline(context);

        Assert.That(context.Trace, Is.EqualTo(new[]
        {
            "logging: started /home",
            "authentication: alice authenticated",
            "terminal",
            "logging: completed /home with OK",
        }));
    }

    [Test]
    public async Task AuthenticationMiddleware_WithoutUser_ShortCircuitsWithUnauthorized()
    {
        var pipeline = new PipelineBuilder()
            .Use(new AuthenticationMiddleware())
            .Use(new LoggingMiddleware())
            .Build(Ok);
        var context = new RequestContext("/home", user: null);

        await pipeline(context);

        Assert.Multiple(() =>
        {
            Assert.That(context.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
            Assert.That(context.Trace, Is.EqualTo(new[] { "authentication: rejected anonymous request" }));
        });
    }

    [Test]
    public void Use_WithNullMiddleware_ThrowsArgumentNullException()
    {
        Assert.That(() => new PipelineBuilder().Use(null!), Throws.ArgumentNullException);
    }
}
