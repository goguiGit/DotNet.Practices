using DotNet.Practices.Abstractions.Creational.FactoryMethod.Interfaces;

namespace DotNet.Practices.Abstractions.Creational.FactoryMethod.Transports;

public sealed class Ship : ITransport
{
    public string Deliver() => "Delivering by sea in a ship";
}
