using DotNet.Practices.Abstractions.Structural.Bridge.Abstracts;
using DotNet.Practices.Abstractions.Structural.Bridge.Interfaces;

namespace DotNet.Practices.Abstractions.Structural.Bridge.Documents;

public sealed class Payslip(IDocumentFormat format, string employee, decimal gross, decimal net) : Document(format)
{
    public override string Render() => string.Join('\n',
        Format.Title($"Payslip {employee}"),
        Format.Line("Gross", FormattableString.Invariant($"{gross:F2}")),
        Format.Line("Net", FormattableString.Invariant($"{net:F2}")));
}
