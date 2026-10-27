using DotNet.Practices.Patterns.Structural.Decorator.Interfaces;

namespace DotNet.Practices.Patterns.Structural.Decorator.Abstracts;

// Forwards by default so each decorator overrides only what it adds.
public abstract class DocumentDecorator(IDocument inner) : IDocument
{
    public virtual string Render() => inner.Render();
}
