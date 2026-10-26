using DotNet.Practices.Abstractions.Veterinary.Interfaces;

namespace DotNet.Practices.Abstractions.Veterinary.Abstracts;

public abstract class Animal : IAnimal
{
    public virtual string Greet() => "Hello";
}
