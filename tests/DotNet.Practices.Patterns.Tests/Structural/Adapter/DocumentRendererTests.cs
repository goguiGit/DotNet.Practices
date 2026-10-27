using DotNet.Practices.Patterns.Structural.Adapter;
using DotNet.Practices.Patterns.Structural.Adapter.Adaptees;
using DotNet.Practices.Patterns.Structural.Adapter.Adapters;
using DotNet.Practices.Patterns.Structural.Adapter.Interfaces;
using DotNet.Practices.Patterns.Structural.Adapter.Renderers;

namespace DotNet.Practices.Patterns.Tests.Structural.Adapter;

public class DocumentRendererTests
{
    private static readonly Document Payslip = new("Payslip March", ["Gross: 2000", "Net: 1600"]);

    [Test]
    public void PlainTextRenderer_Render_ReturnsTitleAndLines()
    {
        IDocumentRenderer renderer = new PlainTextRenderer();

        Assert.That(renderer.Render(Payslip), Is.EqualTo("Payslip March\nGross: 2000\nNet: 1600"));
    }

    [Test]
    public void LegacyPdfEngineAdapter_Render_ReturnsTheEngineOutput()
    {
        IDocumentRenderer renderer = new LegacyPdfEngineAdapter(new LegacyPdfEngine());

        Assert.That(renderer.Render(Payslip), Is.EqualTo("%PDF Payslip March\nGross: 2000\nNet: 1600\n%%EOF"));
    }

    [Test]
    public void LegacyPdfEngineAdapter_Render_StartsAFreshDocumentOnEveryCall()
    {
        IDocumentRenderer renderer = new LegacyPdfEngineAdapter(new LegacyPdfEngine());
        renderer.Render(Payslip);

        Assert.That(renderer.Render(new Document("Empty", [])), Is.EqualTo("%PDF Empty\n%%EOF"));
    }
}
