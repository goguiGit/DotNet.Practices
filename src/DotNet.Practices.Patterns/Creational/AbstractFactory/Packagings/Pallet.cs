using DotNet.Practices.Patterns.Creational.AbstractFactory.Interfaces;

namespace DotNet.Practices.Patterns.Creational.AbstractFactory.Packagings;

public sealed class Pallet : IPackaging
{
    public TransportMode Mode => TransportMode.Road;

    public string Name => "Pallets";

    public bool CanBeLoadedOn(ITransport transport) => transport.Mode == Mode;
}
