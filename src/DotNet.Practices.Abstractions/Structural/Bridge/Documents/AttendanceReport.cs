using DotNet.Practices.Abstractions.Structural.Bridge.Abstracts;
using DotNet.Practices.Abstractions.Structural.Bridge.Interfaces;

namespace DotNet.Practices.Abstractions.Structural.Bridge.Documents;

public sealed class AttendanceReport(IDocumentFormat format, string employee, int hours) : Document(format)
{
    public override string Render() => string.Join('\n',
        Format.Title($"Attendance {employee}"),
        Format.Line("Hours", hours.ToString()));
}
