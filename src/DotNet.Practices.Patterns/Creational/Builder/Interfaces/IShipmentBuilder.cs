namespace DotNet.Practices.Patterns.Creational.Builder.Interfaces;

// Construction steps only. Each concrete builder exposes its own Build(), because the products differ in type.
public interface IShipmentBuilder
{
    IShipmentBuilder Reset();

    IShipmentBuilder From(string origin);

    IShipmentBuilder To(string destination);

    IShipmentBuilder Via(TransportMode mode);

    IShipmentBuilder Insured(decimal value);

    IShipmentBuilder Fragile();

    IShipmentBuilder WithInstructions(string instructions);
}
