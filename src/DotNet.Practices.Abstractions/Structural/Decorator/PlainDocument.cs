using DotNet.Practices.Abstractions.Structural.Decorator.Interfaces;

namespace DotNet.Practices.Abstractions.Structural.Decorator;

public sealed class PlainDocument(string title, string body) : IDocument
{
    public string Render() => $"{title}\n{body}";
}
