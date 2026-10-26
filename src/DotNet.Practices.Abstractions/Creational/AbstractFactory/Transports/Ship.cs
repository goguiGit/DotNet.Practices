using DotNet.Practices.Abstractions.Creational.AbstractFactory.Interfaces;

namespace DotNet.Practices.Abstractions.Creational.AbstractFactory.Transports;

public sealed class Ship : ITransport
{
    public TransportMode Mode => TransportMode.Sea;

    public string Name => "ship";
}
