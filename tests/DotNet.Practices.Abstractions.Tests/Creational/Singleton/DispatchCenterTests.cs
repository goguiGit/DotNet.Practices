using DotNet.Practices.Abstractions.Creational.Singleton;
using DotNet.Practices.Abstractions.Creational.Singleton.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DotNet.Practices.Abstractions.Tests.Creational.Singleton;

public class DispatchCenterTests
{
    [Test]
    public void DispatchCenter_Instance_IsAlwaysTheSameObject()
    {
        Assert.That(DispatchCenter.Instance, Is.SameAs(DispatchCenter.Instance));
    }

    [Test]
    public void DispatchCenter_Next_IncrementsTheProcessWideCounter()
    {
        var first = int.Parse(DispatchCenter.Instance.Next()[4..]);
        var second = int.Parse(DispatchCenter.Instance.Next()[4..]);

        Assert.That(second, Is.EqualTo(first + 1));
    }

    [Test]
    public void TrackingSequence_Next_StartsAtOneForEveryInstance()
    {
        var sequence = new TrackingSequence();

        Assert.That(new[] { sequence.Next(), sequence.Next() }, Is.EqualTo(new[] { "TRK-000001", "TRK-000002" }));
    }

    [Test]
    public void TrackingSequence_RegisteredAsSingleton_IsSharedAcrossScopes()
    {
        using var provider = new ServiceCollection()
            .AddSingleton<ITrackingSequence, TrackingSequence>()
            .BuildServiceProvider();
        using var firstScope = provider.CreateScope();
        using var secondScope = provider.CreateScope();

        Assert.That(
            firstScope.ServiceProvider.GetRequiredService<ITrackingSequence>(),
            Is.SameAs(secondScope.ServiceProvider.GetRequiredService<ITrackingSequence>()));
    }
}
