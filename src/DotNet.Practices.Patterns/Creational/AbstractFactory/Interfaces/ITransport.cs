namespace DotNet.Practices.Patterns.Creational.AbstractFactory.Interfaces;

public interface ITransport
{
    TransportMode Mode { get; }

    string Name { get; }
}
