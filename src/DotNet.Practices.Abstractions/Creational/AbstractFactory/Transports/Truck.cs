using DotNet.Practices.Abstractions.Creational.AbstractFactory.Interfaces;

namespace DotNet.Practices.Abstractions.Creational.AbstractFactory.Transports;

public sealed class Truck : ITransport
{
    public TransportMode Mode => TransportMode.Road;

    public string Name => "truck";
}
