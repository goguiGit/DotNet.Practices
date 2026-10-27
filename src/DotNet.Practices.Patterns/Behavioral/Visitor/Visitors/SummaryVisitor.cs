using DotNet.Practices.Patterns.Behavioral.Visitor.Entries;
using DotNet.Practices.Patterns.Behavioral.Visitor.Interfaces;

namespace DotNet.Practices.Patterns.Behavioral.Visitor.Visitors;

public sealed class SummaryVisitor : ICalendarVisitor<string>
{
    public string Visit(WorkShift shift) => $"Worked {shift.Hours} h";

    public string Visit(Vacation vacation) => $"Vacation, {vacation.Days} day(s)";

    public string Visit(SickLeave sickLeave) =>
        $"Sick leave, {sickLeave.Days} day(s), {(sickLeave.IsCertified ? "certified" : "uncertified")}";
}
