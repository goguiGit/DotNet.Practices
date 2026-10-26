using DotNet.Practices.Abstractions.Interfaces;

namespace DotNet.Practices.Abstractions;

public class Cage
{
    private readonly List<ICageable> _animals = [];

    public Cage(int capacity)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(capacity);
        Capacity = capacity;
    }

    public int Capacity { get; }

    public IReadOnlyList<ICageable> Animals => _animals;

    public void Put(ICageable animal)
    {
        ArgumentNullException.ThrowIfNull(animal);

        if (_animals.Count == Capacity)
        {
            throw new InvalidOperationException($"Cage is full (capacity {Capacity}).");
        }

        _animals.Add(animal);
    }
}
