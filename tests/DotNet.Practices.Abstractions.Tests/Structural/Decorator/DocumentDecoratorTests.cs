using DotNet.Practices.Abstractions.Structural.Decorator;
using DotNet.Practices.Abstractions.Structural.Decorator.Decorators;
using DotNet.Practices.Abstractions.Structural.Decorator.Interfaces;

namespace DotNet.Practices.Abstractions.Tests.Structural.Decorator;

public class DocumentDecoratorTests
{
    [Test]
    public void PlainDocument_Render_ReturnsTitleAndBody()
    {
        IDocument document = new PlainDocument("Payslip Ana", "Net: 1600");

        Assert.That(document.Render(), Is.EqualTo("Payslip Ana\nNet: 1600"));
    }

    [Test]
    public void WatermarkDecorator_Render_AppendsTheWatermarkToTheInnerDocument()
    {
        IDocument document = new WatermarkDecorator(new PlainDocument("Payslip Ana", "Net: 1600"), "CONFIDENTIAL");

        Assert.That(document.Render(), Is.EqualTo("Payslip Ana\nNet: 1600\n[CONFIDENTIAL]"));
    }

    [Test]
    public void Decorators_Stacked_ApplyFromTheInnermostOutwards()
    {
        IDocument document = new SignatureDecorator(
            new WatermarkDecorator(new PlainDocument("Payslip Ana", "Net: 1600"), "CONFIDENTIAL"),
            "HR Department");

        Assert.That(document.Render(), Is.EqualTo("Payslip Ana\nNet: 1600\n[CONFIDENTIAL]\nSigned by HR Department"));
    }
}
