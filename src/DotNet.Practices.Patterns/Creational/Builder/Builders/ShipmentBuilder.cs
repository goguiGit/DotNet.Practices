using DotNet.Practices.Patterns.Creational.Builder.Interfaces;

namespace DotNet.Practices.Patterns.Creational.Builder.Builders;

public sealed class ShipmentBuilder : IShipmentBuilder
{
    private string? _origin;
    private string? _destination;
    private TransportMode _mode;
    private decimal? _insuredValue;
    private bool _isFragile;
    private string? _instructions;

    public IShipmentBuilder Reset()
    {
        _origin = null;
        _destination = null;
        _mode = TransportMode.Road;
        _insuredValue = null;
        _isFragile = false;
        _instructions = null;
        return this;
    }

    public IShipmentBuilder From(string origin)
    {
        _origin = origin;
        return this;
    }

    public IShipmentBuilder To(string destination)
    {
        _destination = destination;
        return this;
    }

    public IShipmentBuilder Via(TransportMode mode)
    {
        _mode = mode;
        return this;
    }

    public IShipmentBuilder Insured(decimal value)
    {
        _insuredValue = value;
        return this;
    }

    public IShipmentBuilder Fragile()
    {
        _isFragile = true;
        return this;
    }

    public IShipmentBuilder WithInstructions(string instructions)
    {
        _instructions = instructions;
        return this;
    }

    public Shipment Build()
    {
        if (_origin is null || _destination is null)
        {
            throw new InvalidOperationException("A shipment needs an origin and a destination.");
        }

        return new Shipment(_origin, _destination, _mode, _insuredValue, _isFragile, _instructions);
    }
}
