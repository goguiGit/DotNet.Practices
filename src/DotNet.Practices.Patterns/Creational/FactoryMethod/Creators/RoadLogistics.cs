using DotNet.Practices.Patterns.Creational.FactoryMethod.Abstracts;
using DotNet.Practices.Patterns.Creational.FactoryMethod.Interfaces;
using DotNet.Practices.Patterns.Creational.FactoryMethod.Transports;

namespace DotNet.Practices.Patterns.Creational.FactoryMethod.Creators;

public sealed class RoadLogistics : Logistics
{
    public override ITransport CreateTransport() => new Truck();
}
