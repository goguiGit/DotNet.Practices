using System.Collections;

namespace DotNet.Practices.Patterns.Behavioral.Iterator;

// Aggregate: callers traverse it without knowing it is a sorted dictionary.
public sealed class WorkCalendar : IEnumerable<CalendarDay>
{
    private readonly SortedDictionary<DateOnly, CalendarDay> _days = [];

    public void Add(CalendarDay day) => _days[day.Date] = day;

    public IEnumerator<CalendarDay> GetEnumerator() => _days.Values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerable<CalendarDay> WorkingDays()
    {
        foreach (var day in this)
        {
            if (day.Kind == DayKind.Working)
            {
                yield return day;
            }
        }
    }
}
