using DotNet.Practices.Patterns.Creational.FactoryMethod.Abstracts;
using DotNet.Practices.Patterns.Creational.FactoryMethod.Creators;
using DotNet.Practices.Patterns.Creational.FactoryMethod.Transports;

namespace DotNet.Practices.Patterns.Tests.Creational.FactoryMethod;

public class LogisticsTests
{
    [Test]
    public void RoadLogistics_CreateTransport_ReturnsTruck()
    {
        var logistics = new RoadLogistics();

        var transport = logistics.CreateTransport();

        Assert.That(transport, Is.InstanceOf<Truck>());
    }

    [Test]
    public void SeaLogistics_CreateTransport_ReturnsShip()
    {
        var logistics = new SeaLogistics();

        var transport = logistics.CreateTransport();

        Assert.That(transport, Is.InstanceOf<Ship>());
    }

    [TestCase(typeof(RoadLogistics), "Planning delivery: Delivering by land in a truck")]
    [TestCase(typeof(SeaLogistics), "Planning delivery: Delivering by sea in a ship")]
    public void PlanDelivery_UsesTheTransportCreatedBySubclass(Type logisticsType, string expected)
    {
        var logistics = (Logistics)Activator.CreateInstance(logisticsType)!;

        var result = logistics.PlanDelivery();

        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void CreateTransport_ReturnsNewInstanceOnEachCall()
    {
        var logistics = new RoadLogistics();

        var first = logistics.CreateTransport();
        var second = logistics.CreateTransport();

        Assert.That(second, Is.Not.SameAs(first));
    }
}
