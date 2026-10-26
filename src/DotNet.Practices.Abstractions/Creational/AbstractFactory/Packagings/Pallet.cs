using DotNet.Practices.Abstractions.Creational.AbstractFactory.Interfaces;

namespace DotNet.Practices.Abstractions.Creational.AbstractFactory.Packagings;

public sealed class Pallet : IPackaging
{
    public TransportMode Mode => TransportMode.Road;

    public string Name => "Pallets";

    public bool CanBeLoadedOn(ITransport transport) => transport.Mode == Mode;
}
