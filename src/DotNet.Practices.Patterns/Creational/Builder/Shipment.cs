namespace DotNet.Practices.Patterns.Creational.Builder;

public sealed record Shipment(
    string Origin,
    string Destination,
    TransportMode Mode,
    decimal? InsuredValue,
    bool IsFragile,
    string? Instructions);
