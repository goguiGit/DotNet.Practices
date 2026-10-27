using System.Collections;

namespace DotNet.Practices.Abstractions.Behavioral.Iterator.Iterators;

// What yield return generates for WorkCalendar.WorkingDays, written by hand to see the pattern.
public sealed class WorkingDayIterator(WorkCalendar calendar) : IEnumerator<CalendarDay>
{
    private readonly IEnumerator<CalendarDay> _days = calendar.GetEnumerator();

    public CalendarDay Current => _days.Current;

    object IEnumerator.Current => Current;

    public bool MoveNext()
    {
        while (_days.MoveNext())
        {
            if (_days.Current.Kind == DayKind.Working)
            {
                return true;
            }
        }

        return false;
    }

    public void Reset() => _days.Reset();

    public void Dispose() => _days.Dispose();
}
