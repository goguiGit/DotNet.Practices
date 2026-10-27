using System.Globalization;
using DotNet.Practices.Abstractions.Creational.Builder.Interfaces;

namespace DotNet.Practices.Abstractions.Creational.Builder.Builders;

// Same steps as ShipmentBuilder, different product: a printable label instead of a domain record.
public sealed class ShippingLabelBuilder : IShipmentBuilder
{
    private readonly List<string> _lines = [];

    public IShipmentBuilder Reset()
    {
        _lines.Clear();
        return this;
    }

    public IShipmentBuilder From(string origin) => Add($"FROM: {origin}");

    public IShipmentBuilder To(string destination) => Add($"TO: {destination}");

    public IShipmentBuilder Via(TransportMode mode) => Add($"VIA: {mode}");

    public IShipmentBuilder Insured(decimal value) => Add($"INSURED: {value.ToString("F2", CultureInfo.InvariantCulture)}");

    public IShipmentBuilder Fragile() => Add("FRAGILE");

    public IShipmentBuilder WithInstructions(string instructions) => Add(instructions);

    public string Build() => string.Join('\n', _lines);

    private IShipmentBuilder Add(string line)
    {
        _lines.Add(line);
        return this;
    }
}
