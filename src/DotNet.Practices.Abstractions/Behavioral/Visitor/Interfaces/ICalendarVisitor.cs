using DotNet.Practices.Abstractions.Behavioral.Visitor.Entries;

namespace DotNet.Practices.Abstractions.Behavioral.Visitor.Interfaces;

// One overload per concrete entry: adding an entry type breaks every visitor on purpose.
public interface ICalendarVisitor<out TResult>
{
    TResult Visit(WorkShift shift);

    TResult Visit(Vacation vacation);

    TResult Visit(SickLeave sickLeave);
}
