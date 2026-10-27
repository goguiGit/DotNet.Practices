namespace DotNet.Practices.Patterns.Creational.Singleton;

// The textbook Singleton. Kept as the counterexample: the dependency is hidden and the counter cannot be reset between tests.
public sealed class DispatchCenter
{
    private static readonly Lazy<DispatchCenter> LazyInstance = new(() => new DispatchCenter());

    private int _last;

    private DispatchCenter()
    {
    }

    public static DispatchCenter Instance => LazyInstance.Value;

    public string Next() => $"TRK-{Interlocked.Increment(ref _last):D6}";
}
