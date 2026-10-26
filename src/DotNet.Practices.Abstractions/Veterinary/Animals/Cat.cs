using DotNet.Practices.Abstractions.Veterinary.Abstracts;
using DotNet.Practices.Abstractions.Veterinary.Interfaces;

namespace DotNet.Practices.Abstractions.Veterinary.Animals;

public class Cat : Animal, ICageable
{
    public override string Greet() => "Meow";
}
