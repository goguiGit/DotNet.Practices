using DotNet.Practices.Abstractions.Interfaces;

namespace DotNet.Practices.Abstractions.Tests;

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
        IAnimal animal = new Fish();

        Assert.That(animal.Greet(), Is.EqualTo("Hello"));
    }

    [Test]
    public void Dog_Greet_ReturnsWoof()
    {
        IAnimal animal = new Dog();

        Assert.That(animal.Greet(), Is.EqualTo("Woof"));
    }

    [Test]
    public void Cat_Greet_ReturnsMeow()
    {
        IAnimal animal = new Cat();

        Assert.That(animal.Greet(), Is.EqualTo("Meow"));
    }
}
