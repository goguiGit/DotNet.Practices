using DotNet.Practices.Abstractions.Creational.AbstractFactory.Interfaces;
using DotNet.Practices.Abstractions.Creational.AbstractFactory.Packagings;
using DotNet.Practices.Abstractions.Creational.AbstractFactory.Transports;

namespace DotNet.Practices.Abstractions.Creational.AbstractFactory.Factories;

public sealed class SeaLogisticsFactory : ILogisticsFactory
{
    public ITransport CreateTransport() => new Ship();

    public IPackaging CreatePackaging() => new Container();
}
