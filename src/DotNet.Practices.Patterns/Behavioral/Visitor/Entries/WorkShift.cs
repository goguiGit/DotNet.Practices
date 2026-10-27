using DotNet.Practices.Patterns.Behavioral.Visitor.Interfaces;

namespace DotNet.Practices.Patterns.Behavioral.Visitor.Entries;

public sealed record WorkShift(decimal Hours) : ICalendarEntry
{
    public TResult Accept<TResult>(ICalendarVisitor<TResult> visitor) => visitor.Visit(this);
}
