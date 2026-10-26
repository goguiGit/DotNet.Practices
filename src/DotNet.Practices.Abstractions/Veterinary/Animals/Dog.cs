using DotNet.Practices.Abstractions.Veterinary.Interfaces;

namespace DotNet.Practices.Abstractions.Veterinary.Animals;

public class Dog : Animal, ICageable
{
    public override string Greet() => "Woof";
}
