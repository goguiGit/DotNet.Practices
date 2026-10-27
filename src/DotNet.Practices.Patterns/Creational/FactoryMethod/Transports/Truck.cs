using DotNet.Practices.Patterns.Creational.FactoryMethod.Interfaces;

namespace DotNet.Practices.Patterns.Creational.FactoryMethod.Transports;

public sealed class Truck : ITransport
{
    public string Deliver() => "Delivering by land in a truck";
}
