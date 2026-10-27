namespace DotNet.Practices.Patterns.Creational.AbstractFactory.Interfaces;

public interface ILogisticsFactory
{
    ITransport CreateTransport();

    IPackaging CreatePackaging();
}
