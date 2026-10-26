using DotNet.Practices.Abstractions.Veterinary.Animals;
using DotNet.Practices.Abstractions.Veterinary;
using DotNet.Practices.Abstractions.Veterinary.Interfaces;

namespace DotNet.Practices.Abstractions.Tests.Veterinary;

public class CageTests
{
    [Test]
    public void Dog_IsCageable()
    {
        Assert.That(new Dog(), Is.InstanceOf<ICageable>());
    }

    [Test]
    public void Cat_IsCageable()
    {
        Assert.That(new Cat(), Is.InstanceOf<ICageable>());
    }

    [Test]
    public void Horse_IsNotCageable()
    {
        Assert.That(new Horse(), Is.Not.InstanceOf<ICageable>());
    }

    [Test]
    public void Cage_Put_AddsAnimal()
    {
        var cage = new Cage(capacity: 2);
        var dog = new Dog();

        cage.Put(dog);

        Assert.That(cage.Animals, Is.EqualTo(new[] { dog }));
    }

    [Test]
    public void Cage_Put_WhenFull_ThrowsInvalidOperationException()
    {
        var cage = new Cage(capacity: 1);
        cage.Put(new Dog());

        Assert.That(() => cage.Put(new Cat()), Throws.InvalidOperationException);
    }

    [Test]
    public void Cage_Constructor_WithNonPositiveCapacity_ThrowsArgumentOutOfRangeException()
    {
        Assert.That(() => new Cage(capacity: 0), Throws.TypeOf<ArgumentOutOfRangeException>());
    }
}
