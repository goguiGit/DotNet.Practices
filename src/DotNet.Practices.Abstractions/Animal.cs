using DotNet.Practices.Abstractions.Interfaces;

namespace DotNet.Practices.Abstractions;

public abstract class Animal : IAnimal
{
    public virtual string Greet() => "Hello";
}
