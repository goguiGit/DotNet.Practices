using DotNet.Practices.Abstractions.Behavioral.Iterator;
using DotNet.Practices.Abstractions.Behavioral.Iterator.Iterators;

namespace DotNet.Practices.Abstractions.Tests.Behavioral.Iterator;

public class WorkCalendarTests
{
    private static readonly CalendarDay Monday = new(new DateOnly(2026, 10, 5), DayKind.Working);
    private static readonly CalendarDay Tuesday = new(new DateOnly(2026, 10, 6), DayKind.Working);
    private static readonly CalendarDay Holiday = new(new DateOnly(2026, 10, 12), DayKind.Holiday);
    private static readonly CalendarDay Saturday = new(new DateOnly(2026, 10, 10), DayKind.Weekend);

    private static WorkCalendar CalendarAddedOutOfOrder()
    {
        var calendar = new WorkCalendar();
        calendar.Add(Holiday);
        calendar.Add(Tuesday);
        calendar.Add(Saturday);
        calendar.Add(Monday);
        return calendar;
    }

    [Test]
    public void WorkCalendar_Enumerate_ReturnsDaysInDateOrderWhateverTheInsertionOrder()
    {
        Assert.That(CalendarAddedOutOfOrder(), Is.EqualTo(new[] { Monday, Tuesday, Saturday, Holiday }));
    }

    [Test]
    public void WorkCalendar_WorkingDays_SkipsWeekendsAndHolidays()
    {
        Assert.That(CalendarAddedOutOfOrder().WorkingDays(), Is.EqualTo(new[] { Monday, Tuesday }));
    }

    [Test]
    public void WorkingDayIterator_MoveNext_WalksTheSameDaysAsTheYieldVersion()
    {
        var calendar = CalendarAddedOutOfOrder();
        using var iterator = new WorkingDayIterator(calendar);
        var walked = new List<CalendarDay>();
        while (iterator.MoveNext())
        {
            walked.Add(iterator.Current);
        }

        Assert.That(walked, Is.EqualTo(calendar.WorkingDays()));
    }
}
