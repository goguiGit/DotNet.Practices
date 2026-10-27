using DotNet.Practices.Abstractions.Structural.Bridge.Interfaces;

namespace DotNet.Practices.Abstractions.Structural.Bridge.Abstracts;

// Abstraction side of the bridge: what a document says, delegating how it looks to the format.
public abstract class Document(IDocumentFormat format)
{
    protected IDocumentFormat Format { get; } = format;

    public abstract string Render();
}
