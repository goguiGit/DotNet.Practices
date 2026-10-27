using DotNet.Practices.Abstractions.Creational.Singleton.Interfaces;

namespace DotNet.Practices.Abstractions.Creational.Singleton;

// Same behaviour, no static state: "only one" is decided by whoever registers it (AddSingleton), not by the class.
public sealed class TrackingSequence : ITrackingSequence
{
    private int _last;

    public string Next() => $"TRK-{Interlocked.Increment(ref _last):D6}";
}
