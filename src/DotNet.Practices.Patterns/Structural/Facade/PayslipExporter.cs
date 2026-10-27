using DotNet.Practices.Patterns.Structural.Facade.Services;

namespace DotNet.Practices.Patterns.Structural.Facade;

// One business operation over three services; callers never touch the services themselves.
public sealed class PayslipExporter(DocumentRenderer renderer, DocumentArchive archive, EmployeeNotifier notifier)
{
    public string Export(string employee, decimal net)
    {
        var reference = $"{employee}-payslip";
        var content = renderer.Render($"Payslip {employee}", FormattableString.Invariant($"Net: {net:F2}"));

        archive.Store(reference, content);
        notifier.Notify(employee, $"your payslip is ready ({reference})");

        return reference;
    }
}
