using DotNet.Practices.Abstractions.Behavioral.Visitor.Entries;
using DotNet.Practices.Abstractions.Behavioral.Visitor.Interfaces;
using DotNet.Practices.Abstractions.Behavioral.Visitor.Visitors;

namespace DotNet.Practices.Abstractions.Tests.Behavioral.Visitor;

public class CalendarVisitorTests
{
    [Test]
    public void PaidHoursVisitor_WorkShift_PaysEveryHour()
    {
        ICalendarEntry shift = new WorkShift(Hours: 7.5m);

        Assert.That(shift.Accept(new PaidHoursVisitor()), Is.EqualTo(7.5m));
    }

    [Test]
    public void PaidHoursVisitor_Vacation_PaysEightHoursPerDay()
    {
        ICalendarEntry vacation = new Vacation(Days: 2);

        Assert.That(vacation.Accept(new PaidHoursVisitor()), Is.EqualTo(16m));
    }

    [Test]
    public void PaidHoursVisitor_UncertifiedSickLeave_PaysSixtyPercent()
    {
        ICalendarEntry sickLeave = new SickLeave(Days: 1, IsCertified: false);

        Assert.That(sickLeave.Accept(new PaidHoursVisitor()), Is.EqualTo(4.8m));
    }

    [Test]
    public void SummaryVisitor_Accept_DescribesEveryEntryWithoutChangingTheEntries()
    {
        ICalendarEntry[] month = [new WorkShift(8m), new Vacation(1), new SickLeave(2, IsCertified: true)];

        var lines = month.Select(entry => entry.Accept(new SummaryVisitor()));

        Assert.That(lines, Is.EqualTo(new[] { "Worked 8 h", "Vacation, 1 day(s)", "Sick leave, 2 day(s), certified" }));
    }
}
