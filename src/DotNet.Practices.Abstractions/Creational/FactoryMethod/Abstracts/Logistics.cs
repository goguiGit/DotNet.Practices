using DotNet.Practices.Abstractions.Creational.FactoryMethod.Interfaces;

namespace DotNet.Practices.Abstractions.Creational.FactoryMethod.Abstracts;

public abstract class Logistics
{
    // The factory method: subclasses decide which transport to build.
    public abstract ITransport CreateTransport();

    // Business logic written once against the abstraction, never against Truck or Ship.
    public string PlanDelivery() => $"Planning delivery: {CreateTransport().Deliver()}";
}
