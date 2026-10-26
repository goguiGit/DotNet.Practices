using DotNet.Practices.Abstractions.Creational.FactoryMethod.Abstracts;
using DotNet.Practices.Abstractions.Creational.FactoryMethod.Interfaces;
using DotNet.Practices.Abstractions.Creational.FactoryMethod.Transports;

namespace DotNet.Practices.Abstractions.Creational.FactoryMethod.Creators;

public sealed class RoadLogistics : Logistics
{
    public override ITransport CreateTransport() => new Truck();
}
