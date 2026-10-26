using DotNet.Practices.Abstractions.Veterinary.Abstracts;
namespace DotNet.Practices.Abstractions.Veterinary.Animals;

public class Horse : Animal
{
    public override string Greet() => "Neigh";
}
