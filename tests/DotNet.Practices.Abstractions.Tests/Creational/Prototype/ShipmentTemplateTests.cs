using DotNet.Practices.Abstractions.Creational.Prototype;
using DotNet.Practices.Abstractions.Creational.Prototype.Abstracts;
using DotNet.Practices.Abstractions.Creational.Prototype.Templates;

namespace DotNet.Practices.Abstractions.Tests.Creational.Prototype;

public class ShipmentTemplateTests
{
    [Test]
    public void RoadShipment_Clone_ReturnsNewInstance()
    {
        var template = new RoadShipment("Madrid", "Valencia", "Refrigerated truck");

        Assert.That(template.Clone(), Is.Not.SameAs(template));
    }

    [Test]
    public void RoadShipment_Clone_CopiesEveryField()
    {
        var template = new RoadShipment("Madrid", "Valencia", "Refrigerated truck");
        template.Stops.Add("Cuenca");

        var clone = (RoadShipment)template.Clone();

        Assert.That(clone, Has.Property("Origin").EqualTo("Madrid")
            .And.Property("Destination").EqualTo("Valencia")
            .And.Property("TruckType").EqualTo("Refrigerated truck")
            .And.Property("Stops").EqualTo(new[] { "Cuenca" }));
    }

    [Test]
    public void ShipmentTemplate_Clone_DoesNotShareStopsWithTheTemplate()
    {
        var template = new SeaShipment("Valencia", "Palma", 2);
        template.Stops.Add("Ibiza");

        var clone = template.Clone();
        clone.Stops.Add("Formentera");

        Assert.That(template.Stops, Is.EqualTo(new[] { "Ibiza" }));
    }

    [Test]
    public void ShipmentTemplate_Clone_KeepsTheConcreteTypeWithoutTheCallerKnowingIt()
    {
        ShipmentTemplate template = new SeaShipment("Valencia", "Palma", 2);

        Assert.That(template.Clone(), Is.InstanceOf<SeaShipment>());
    }

    [Test]
    public void ShipmentTemplates_Create_ReturnsAFreshCopyEachTime()
    {
        var templates = new ShipmentTemplates();
        templates.Register("weekly-palma", new SeaShipment("Valencia", "Palma", 2));

        Assert.That(templates.Create("weekly-palma"), Is.Not.SameAs(templates.Create("weekly-palma")));
    }

    [Test]
    public void ShipmentTemplates_Create_UnknownName_Throws()
    {
        var templates = new ShipmentTemplates();

        Assert.That(() => templates.Create("nope"), Throws.TypeOf<KeyNotFoundException>());
    }
}
