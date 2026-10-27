using DotNet.Practices.Patterns.Structural.Facade;
using DotNet.Practices.Patterns.Structural.Facade.Services;

namespace DotNet.Practices.Patterns.Tests.Structural.Facade;

public class PayslipExporterTests
{
    [Test]
    public void PayslipExporter_Export_ArchivesTheRenderedPayslipUnderTheEmployeeName()
    {
        var archive = new DocumentArchive();
        var exporter = new PayslipExporter(new DocumentRenderer(), archive, new EmployeeNotifier());

        exporter.Export("Ana", 1600m);

        Assert.That(archive.Read("Ana-payslip"), Is.EqualTo("Payslip Ana\nNet: 1600.00"));
    }

    [Test]
    public void PayslipExporter_Export_NotifiesTheEmployeeOnce()
    {
        var notifier = new EmployeeNotifier();
        var exporter = new PayslipExporter(new DocumentRenderer(), new DocumentArchive(), notifier);

        exporter.Export("Ana", 1600m);

        Assert.That(notifier.Sent, Is.EqualTo(new[] { "Ana: your payslip is ready (Ana-payslip)" }));
    }

    [Test]
    public void PayslipExporter_Export_ReturnsTheArchiveReference()
    {
        var exporter = new PayslipExporter(new DocumentRenderer(), new DocumentArchive(), new EmployeeNotifier());

        Assert.That(exporter.Export("Ana", 1600m), Is.EqualTo("Ana-payslip"));
    }
}
