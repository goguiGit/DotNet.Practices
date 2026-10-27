namespace DotNet.Practices.Patterns.Creational.Prototype.Abstracts;

public abstract class ShipmentTemplate
{
    protected ShipmentTemplate(string origin, string destination)
    {
        Origin = origin;
        Destination = destination;
        Stops = [];
    }

    // Copy constructor: the one place that knows which fields are shared and which must be duplicated.
    protected ShipmentTemplate(ShipmentTemplate source)
    {
        Origin = source.Origin;
        Destination = source.Destination;
        Stops = [.. source.Stops];
    }

    public string Origin { get; }

    public string Destination { get; }

    public ICollection<string> Stops { get; }

    public abstract ShipmentTemplate Clone();
}
