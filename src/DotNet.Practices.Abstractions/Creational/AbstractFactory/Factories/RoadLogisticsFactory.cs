using DotNet.Practices.Abstractions.Creational.AbstractFactory.Interfaces;
using DotNet.Practices.Abstractions.Creational.AbstractFactory.Packagings;
using DotNet.Practices.Abstractions.Creational.AbstractFactory.Transports;

namespace DotNet.Practices.Abstractions.Creational.AbstractFactory.Factories;

public sealed class RoadLogisticsFactory : ILogisticsFactory
{
    public ITransport CreateTransport() => new Truck();

    public IPackaging CreatePackaging() => new Pallet();
}
