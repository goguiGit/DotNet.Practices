using DotNet.Practices.Abstractions.Creational.AbstractFactory.Factories;
using DotNet.Practices.Abstractions.Creational.AbstractFactory.Interfaces;

namespace DotNet.Practices.Abstractions.Creational.AbstractFactory;

// The one place that knows every concrete factory. In a DI app this is the registration code.
public static class LogisticsFactories
{
    public static ILogisticsFactory For(TransportMode mode) => mode switch
    {
        TransportMode.Road => new RoadLogisticsFactory(),
        TransportMode.Sea => new SeaLogisticsFactory(),
        _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, "Unknown transport mode."),
    };
}
