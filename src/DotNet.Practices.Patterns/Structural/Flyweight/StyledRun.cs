namespace DotNet.Practices.Patterns.Structural.Flyweight;

// Extrinsic state (the text) plus a reference to the shared style.
public sealed record StyledRun(string Text, TextStyle Style);
