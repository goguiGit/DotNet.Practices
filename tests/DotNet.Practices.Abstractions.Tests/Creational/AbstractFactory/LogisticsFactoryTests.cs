using DotNet.Practices.Abstractions.Creational.AbstractFactory;
using DotNet.Practices.Abstractions.Creational.AbstractFactory.Factories;
using DotNet.Practices.Abstractions.Creational.AbstractFactory.Packagings;
using DotNet.Practices.Abstractions.Creational.AbstractFactory.Transports;

namespace DotNet.Practices.Abstractions.Tests.Creational.AbstractFactory;

public class LogisticsFactoryTests
{
    [Test]
    public void RoadFactory_CreatesTruckAndPallet()
    {
        var factory = new RoadLogisticsFactory();

        Assert.Multiple(() =>
        {
            Assert.That(factory.CreateTransport(), Is.InstanceOf<Truck>());
            Assert.That(factory.CreatePackaging(), Is.InstanceOf<Pallet>());
        });
    }

    [Test]
    public void SeaFactory_CreatesShipAndContainer()
    {
        var factory = new SeaLogisticsFactory();

        Assert.Multiple(() =>
        {
            Assert.That(factory.CreateTransport(), Is.InstanceOf<Ship>());
            Assert.That(factory.CreatePackaging(), Is.InstanceOf<Container>());
        });
    }

    [Test]
    public void ProductsFromSameFactory_AreCompatible()
    {
        var factory = new SeaLogisticsFactory();

        var transport = factory.CreateTransport();
        var packaging = factory.CreatePackaging();

        Assert.That(packaging.CanBeLoadedOn(transport), Is.True);
    }

    [Test]
    public void ProductsFromDifferentFactories_AreNotCompatible()
    {
        var truck = new RoadLogisticsFactory().CreateTransport();
        var container = new SeaLogisticsFactory().CreatePackaging();

        Assert.That(container.CanBeLoadedOn(truck), Is.False);
    }

    [TestCase(TransportMode.Road, "Pallets loaded on a truck")]
    [TestCase(TransportMode.Sea, "Containers loaded on a ship")]
    public void ShipmentPlanner_Plan_UsesOnlyTheGivenFactory(TransportMode mode, string expected)
    {
        var planner = new ShipmentPlanner(LogisticsFactories.For(mode));

        Assert.That(planner.Plan(), Is.EqualTo(expected));
    }
}
