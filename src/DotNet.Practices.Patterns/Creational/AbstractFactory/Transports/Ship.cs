using DotNet.Practices.Patterns.Creational.AbstractFactory.Interfaces;

namespace DotNet.Practices.Patterns.Creational.AbstractFactory.Transports;

public sealed class Ship : ITransport
{
    public TransportMode Mode => TransportMode.Sea;

    public string Name => "ship";
}
