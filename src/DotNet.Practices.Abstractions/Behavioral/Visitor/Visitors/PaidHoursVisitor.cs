using DotNet.Practices.Abstractions.Behavioral.Visitor.Entries;
using DotNet.Practices.Abstractions.Behavioral.Visitor.Interfaces;

namespace DotNet.Practices.Abstractions.Behavioral.Visitor.Visitors;

public sealed class PaidHoursVisitor : ICalendarVisitor<decimal>
{
    private const decimal HoursPerDay = 8m;

    public decimal Visit(WorkShift shift) => shift.Hours;

    public decimal Visit(Vacation vacation) => vacation.Days * HoursPerDay;

    public decimal Visit(SickLeave sickLeave) => sickLeave.Days * HoursPerDay * (sickLeave.IsCertified ? 1m : 0.6m);
}
