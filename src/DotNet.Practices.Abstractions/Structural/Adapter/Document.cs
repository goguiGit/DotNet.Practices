namespace DotNet.Practices.Abstractions.Structural.Adapter;

public sealed record Document(string Title, IReadOnlyList<string> Lines);
