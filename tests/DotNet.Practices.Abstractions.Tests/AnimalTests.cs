namespace DotNet.Practices.Abstractions.Tests;

public class AnimalTests
{
    [Test]
    public void Dog_Greet_ReturnsWoof()
    {
        Animal animal = new Dog();

        Assert.That(animal.Greet(), Is.EqualTo("Woof"));
    }

    [Test]
    public void Cat_Greet_ReturnsMeow()
    {
        Animal animal = new Cat();

        Assert.That(animal.Greet(), Is.EqualTo("Meow"));
    }

    [Test]
    public void Animal_IsAbstract()
    {
        Assert.That(typeof(Animal).IsAbstract, Is.True);
    }
}
