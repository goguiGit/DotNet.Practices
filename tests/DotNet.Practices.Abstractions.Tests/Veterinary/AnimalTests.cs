using DotNet.Practices.Abstractions.Veterinary.Abstracts;
using DotNet.Practices.Abstractions.Veterinary.Animals;
using DotNet.Practices.Abstractions.Veterinary;
using DotNet.Practices.Abstractions.Veterinary.Interfaces;

namespace DotNet.Practices.Abstractions.Tests.Veterinary;

public class AnimalTests
{
    private sealed class Fish : Animal;

    [Test]
    public void Animal_ImplementsIAnimal()
    {
        Assert.That(typeof(IAnimal).IsAssignableFrom(typeof(Animal)), Is.True);
    }

    [Test]
    public void Animal_IsAbstract()
    {
        Assert.That(typeof(Animal).IsAbstract, Is.True);
    }

    [Test]
    public void Animal_Greet_ReturnsDefaultGreetingWhenNotOverridden()
    {
        var animal = new Fish();

        Assert.That(animal.Greet(), Is.EqualTo("Hello"));
    }

    [Test]
    public void Dog_Greet_ReturnsWoof()
    {
        var animal = new Dog();

        Assert.That(animal.Greet(), Is.EqualTo("Woof"));
    }

    [Test]
    public void Cat_Greet_ReturnsMeow()
    {
        var animal = new Cat();

        Assert.That(animal.Greet(), Is.EqualTo("Meow"));
    }
}
