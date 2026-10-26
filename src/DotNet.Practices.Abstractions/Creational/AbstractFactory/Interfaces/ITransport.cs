namespace DotNet.Practices.Abstractions.Creational.AbstractFactory.Interfaces;

public interface ITransport
{
    TransportMode Mode { get; }

    string Name { get; }
}
