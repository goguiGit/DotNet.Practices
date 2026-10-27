using DotNet.Practices.Abstractions.Creational.Prototype.Abstracts;

namespace DotNet.Practices.Abstractions.Creational.Prototype.Templates;

public sealed class SeaShipment : ShipmentTemplate
{
    public SeaShipment(string origin, string destination, int containers)
        : base(origin, destination)
    {
        Containers = containers;
    }

    private SeaShipment(SeaShipment source)
        : base(source)
    {
        Containers = source.Containers;
    }

    public int Containers { get; }

    public override ShipmentTemplate Clone() => new SeaShipment(this);
}
