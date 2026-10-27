namespace DotNet.Practices.Patterns.Structural.Flyweight;

// Intrinsic state: immutable, so one instance can be shared by every run that uses it.
public sealed record TextStyle(string Font, int Size, bool IsBold);
