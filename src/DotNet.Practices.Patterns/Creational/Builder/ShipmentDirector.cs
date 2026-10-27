using DotNet.Practices.Patterns.Creational.Builder.Interfaces;

namespace DotNet.Practices.Patterns.Creational.Builder;

// Recipes: the order of steps for the shipments the business asks for by name.
public sealed class ShipmentDirector
{
    public void StandardDelivery(IShipmentBuilder builder, string origin, string destination) =>
        builder.Reset().From(origin).To(destination).Via(TransportMode.Road);

    public void FragileSeaFreight(IShipmentBuilder builder, string origin, string destination, decimal insuredValue) =>
        builder.Reset().From(origin).To(destination).Via(TransportMode.Sea).Insured(insuredValue).Fragile().WithInstructions("Keep dry");
}
