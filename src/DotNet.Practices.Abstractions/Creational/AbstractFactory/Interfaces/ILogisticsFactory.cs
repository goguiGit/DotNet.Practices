namespace DotNet.Practices.Abstractions.Creational.AbstractFactory.Interfaces;

public interface ILogisticsFactory
{
    ITransport CreateTransport();

    IPackaging CreatePackaging();
}
