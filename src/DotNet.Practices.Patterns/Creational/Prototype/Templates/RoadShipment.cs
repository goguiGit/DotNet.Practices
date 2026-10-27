using DotNet.Practices.Patterns.Creational.Prototype.Abstracts;

namespace DotNet.Practices.Patterns.Creational.Prototype.Templates;

public sealed class RoadShipment : ShipmentTemplate
{
    public RoadShipment(string origin, string destination, string truckType)
        : base(origin, destination)
    {
        TruckType = truckType;
    }

    private RoadShipment(RoadShipment source)
        : base(source)
    {
        TruckType = source.TruckType;
    }

    public string TruckType { get; }

    public override ShipmentTemplate Clone() => new RoadShipment(this);
}
