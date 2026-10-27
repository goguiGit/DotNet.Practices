using DotNet.Practices.Abstractions.Structural.Flyweight;

namespace DotNet.Practices.Abstractions.Tests.Structural.Flyweight;

public class TextStyleFactoryTests
{
    [Test]
    public void TextStyleFactory_Get_ReturnsTheSameInstanceForTheSameStyle()
    {
        var styles = new TextStyleFactory();

        Assert.That(styles.Get("Arial", 12, isBold: false), Is.SameAs(styles.Get("Arial", 12, isBold: false)));
    }

    [Test]
    public void TextStyleFactory_Get_ReturnsADifferentInstanceForADifferentStyle()
    {
        var styles = new TextStyleFactory();

        Assert.That(styles.Get("Arial", 12, isBold: false), Is.Not.SameAs(styles.Get("Arial", 12, isBold: true)));
    }

    [Test]
    public void DocumentPage_Write_SharesStylesAcrossThousandsOfRuns()
    {
        var styles = new TextStyleFactory();
        var page = new DocumentPage(styles);

        for (var i = 0; i < 1000; i++)
        {
            page.Write($"Line {i}", "Arial", 12, isBold: i % 10 == 0);
        }

        Assert.That(styles.Count, Is.EqualTo(2));
    }

    [Test]
    public void DocumentPage_Write_KeepsTheTextOutsideTheSharedStyle()
    {
        var page = new DocumentPage(new TextStyleFactory());
        page.Write("Payslip Ana", "Arial", 14, isBold: true);

        Assert.That(page.Runs, Is.EqualTo(new[] { new StyledRun("Payslip Ana", new TextStyle("Arial", 14, true)) }));
    }
}
