namespace DotNet.Practices.Patterns.Creational.AbstractFactory.Interfaces;

public interface IPackaging
{
    TransportMode Mode { get; }

    string Name { get; }

    // Products of one family work together; that compatibility is what the factory guarantees.
    bool CanBeLoadedOn(ITransport transport);
}
