using DotNet.Practices.Abstractions.Creational.FactoryMethod.Interfaces;

namespace DotNet.Practices.Abstractions.Creational.FactoryMethod.Transports;

public sealed class Truck : ITransport
{
    public string Deliver() => "Delivering by land in a truck";
}
