using DotNet.Practices.Patterns.Creational.AbstractFactory.Interfaces;

namespace DotNet.Practices.Patterns.Creational.AbstractFactory.Transports;

public sealed class Truck : ITransport
{
    public TransportMode Mode => TransportMode.Road;

    public string Name => "truck";
}
