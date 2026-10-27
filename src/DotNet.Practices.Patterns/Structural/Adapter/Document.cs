namespace DotNet.Practices.Patterns.Structural.Adapter;

public sealed record Document(string Title, IReadOnlyList<string> Lines);
