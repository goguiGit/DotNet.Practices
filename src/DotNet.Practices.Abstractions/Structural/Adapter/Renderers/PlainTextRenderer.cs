using DotNet.Practices.Abstractions.Structural.Adapter.Interfaces;

namespace DotNet.Practices.Abstractions.Structural.Adapter.Renderers;

public sealed class PlainTextRenderer : IDocumentRenderer
{
    public string Render(Document document) => string.Join('\n', document.Lines.Prepend(document.Title));
}
