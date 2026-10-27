using DotNet.Practices.Patterns.Structural.Decorator.Interfaces;

namespace DotNet.Practices.Patterns.Structural.Decorator;

public sealed class PlainDocument(string title, string body) : IDocument
{
    public string Render() => $"{title}\n{body}";
}
