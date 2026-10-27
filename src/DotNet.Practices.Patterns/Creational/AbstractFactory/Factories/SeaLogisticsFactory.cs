using DotNet.Practices.Patterns.Creational.AbstractFactory.Interfaces;
using DotNet.Practices.Patterns.Creational.AbstractFactory.Packagings;
using DotNet.Practices.Patterns.Creational.AbstractFactory.Transports;

namespace DotNet.Practices.Patterns.Creational.AbstractFactory.Factories;

public sealed class SeaLogisticsFactory : ILogisticsFactory
{
    public ITransport CreateTransport() => new Ship();

    public IPackaging CreatePackaging() => new Container();
}
