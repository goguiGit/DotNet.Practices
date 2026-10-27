using DotNet.Practices.Patterns.Structural.Bridge.Documents;
using DotNet.Practices.Patterns.Structural.Bridge.Formats;

namespace DotNet.Practices.Patterns.Tests.Structural.Bridge;

public class DocumentTests
{
    [Test]
    public void Payslip_Render_UsesTheMarkdownFormat()
    {
        var payslip = new Payslip(new MarkdownFormat(), "Ana", 2000m, 1600m);

        Assert.That(payslip.Render(), Is.EqualTo("# Payslip Ana\n**Gross**: 2000.00\n**Net**: 1600.00"));
    }

    [Test]
    public void Payslip_Render_UsesTheHtmlFormat()
    {
        var payslip = new Payslip(new HtmlFormat(), "Ana", 2000m, 1600m);

        Assert.That(payslip.Render(), Is.EqualTo("<h1>Payslip Ana</h1>\n<p>Gross: 2000.00</p>\n<p>Net: 1600.00</p>"));
    }

    [Test]
    public void AttendanceReport_Render_UsesTheSameFormatWithoutANewClass()
    {
        var report = new AttendanceReport(new HtmlFormat(), "Ana", 160);

        Assert.That(report.Render(), Is.EqualTo("<h1>Attendance Ana</h1>\n<p>Hours: 160</p>"));
    }
}
