using DotNet.Practices.Patterns.Creational.FactoryMethod.Interfaces;

namespace DotNet.Practices.Patterns.Creational.FactoryMethod.Transports;

public sealed class Ship : ITransport
{
    public string Deliver() => "Delivering by sea in a ship";
}
