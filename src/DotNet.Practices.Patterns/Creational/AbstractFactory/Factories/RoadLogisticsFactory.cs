using DotNet.Practices.Patterns.Creational.AbstractFactory.Interfaces;
using DotNet.Practices.Patterns.Creational.AbstractFactory.Packagings;
using DotNet.Practices.Patterns.Creational.AbstractFactory.Transports;

namespace DotNet.Practices.Patterns.Creational.AbstractFactory.Factories;

public sealed class RoadLogisticsFactory : ILogisticsFactory
{
    public ITransport CreateTransport() => new Truck();

    public IPackaging CreatePackaging() => new Pallet();
}
