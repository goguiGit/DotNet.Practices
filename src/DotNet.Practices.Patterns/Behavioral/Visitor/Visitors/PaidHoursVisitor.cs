using DotNet.Practices.Patterns.Behavioral.Visitor.Entries;
using DotNet.Practices.Patterns.Behavioral.Visitor.Interfaces;

namespace DotNet.Practices.Patterns.Behavioral.Visitor.Visitors;

public sealed class PaidHoursVisitor : ICalendarVisitor<decimal>
{
    private const decimal HoursPerDay = 8m;

    public decimal Visit(WorkShift shift) => shift.Hours;

    public decimal Visit(Vacation vacation) => vacation.Days * HoursPerDay;

    public decimal Visit(SickLeave sickLeave) => sickLeave.Days * HoursPerDay * (sickLeave.IsCertified ? 1m : 0.6m);
}
