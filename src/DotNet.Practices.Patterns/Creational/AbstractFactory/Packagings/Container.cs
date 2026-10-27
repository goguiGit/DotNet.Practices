using DotNet.Practices.Patterns.Creational.AbstractFactory.Interfaces;

namespace DotNet.Practices.Patterns.Creational.AbstractFactory.Packagings;

public sealed class Container : IPackaging
{
    public TransportMode Mode => TransportMode.Sea;

    public string Name => "Containers";

    public bool CanBeLoadedOn(ITransport transport) => transport.Mode == Mode;
}
