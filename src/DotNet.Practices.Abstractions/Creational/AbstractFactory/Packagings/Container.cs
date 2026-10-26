using DotNet.Practices.Abstractions.Creational.AbstractFactory.Interfaces;

namespace DotNet.Practices.Abstractions.Creational.AbstractFactory.Packagings;

public sealed class Container : IPackaging
{
    public TransportMode Mode => TransportMode.Sea;

    public string Name => "Containers";

    public bool CanBeLoadedOn(ITransport transport) => transport.Mode == Mode;
}
