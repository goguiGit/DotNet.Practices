using DotNet.Practices.Patterns.Creational.AbstractFactory.Interfaces;

namespace DotNet.Practices.Patterns.Creational.AbstractFactory;

// Client code: depends on the abstract factory and abstract products only.
public sealed class ShipmentPlanner(ILogisticsFactory factory)
{
    public string Plan()
    {
        var transport = factory.CreateTransport();
        var packaging = factory.CreatePackaging();

        return $"{packaging.Name} loaded on a {transport.Name}";
    }
}
