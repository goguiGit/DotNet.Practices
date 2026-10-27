using DotNet.Practices.Abstractions.Creational.Builder;
using DotNet.Practices.Abstractions.Creational.Builder.Builders;

namespace DotNet.Practices.Abstractions.Tests.Creational.Builder;

public class ShipmentBuilderTests
{
    [Test]
    public void ShipmentBuilder_Build_ReturnsShipmentWithEveryStep()
    {
        var builder = new ShipmentBuilder();
        builder.From("Madrid").To("Valencia").Via(TransportMode.Sea).Insured(1200m).Fragile().WithInstructions("Keep dry");

        var shipment = builder.Build();

        Assert.That(shipment, Is.EqualTo(new Shipment("Madrid", "Valencia", TransportMode.Sea, 1200m, true, "Keep dry")));
    }

    [Test]
    public void ShipmentBuilder_Build_DefaultsOptionalSteps()
    {
        var builder = new ShipmentBuilder();
        builder.From("Madrid").To("Valencia");

        var shipment = builder.Build();

        Assert.That(shipment, Is.EqualTo(new Shipment("Madrid", "Valencia", TransportMode.Road, null, false, null)));
    }

    [Test]
    public void ShipmentBuilder_Build_WithoutDestination_Throws()
    {
        var builder = new ShipmentBuilder();
        builder.From("Madrid");

        Assert.That(() => builder.Build(), Throws.InvalidOperationException);
    }

    [Test]
    public void ShippingLabelBuilder_Build_ReturnsOneLinePerStep()
    {
        var builder = new ShippingLabelBuilder();
        builder.From("Madrid").To("Valencia").Via(TransportMode.Sea).Insured(1200m).Fragile().WithInstructions("Keep dry");

        var label = builder.Build();

        Assert.That(label, Is.EqualTo("FROM: Madrid\nTO: Valencia\nVIA: Sea\nINSURED: 1200.00\nFRAGILE\nKeep dry"));
    }

    [Test]
    public void ShipmentDirector_FragileSeaFreight_BuildsShipment()
    {
        var builder = new ShipmentBuilder();
        new ShipmentDirector().FragileSeaFreight(builder, "Madrid", "Palma", 500m);

        Assert.That(builder.Build(), Is.EqualTo(new Shipment("Madrid", "Palma", TransportMode.Sea, 500m, true, "Keep dry")));
    }

    [Test]
    public void ShipmentDirector_FragileSeaFreight_BuildsLabelWithTheSameSteps()
    {
        var builder = new ShippingLabelBuilder();
        new ShipmentDirector().FragileSeaFreight(builder, "Madrid", "Palma", 500m);

        Assert.That(builder.Build(), Is.EqualTo("FROM: Madrid\nTO: Palma\nVIA: Sea\nINSURED: 500.00\nFRAGILE\nKeep dry"));
    }

    [Test]
    public void ShipmentDirector_StandardDelivery_ResetsStepsFromPreviousBuild()
    {
        var builder = new ShipmentBuilder();
        builder.Fragile().Insured(999m);
        new ShipmentDirector().StandardDelivery(builder, "Madrid", "Sevilla");

        Assert.That(builder.Build(), Is.EqualTo(new Shipment("Madrid", "Sevilla", TransportMode.Road, null, false, null)));
    }
}
