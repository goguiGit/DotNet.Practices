using DotNet.Practices.Abstractions.Structural.Adapter.Adaptees;
using DotNet.Practices.Abstractions.Structural.Adapter.Interfaces;

namespace DotNet.Practices.Abstractions.Structural.Adapter.Adapters;

public sealed class LegacyPdfEngineAdapter(LegacyPdfEngine engine) : IDocumentRenderer
{
    public string Render(Document document)
    {
        engine.Open(document.Title);
        foreach (var line in document.Lines)
        {
            engine.WriteLine(line);
        }

        return engine.Close();
    }
}
