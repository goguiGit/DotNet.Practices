using DotNet.Practices.Patterns.Behavioral.Visitor.Interfaces;

namespace DotNet.Practices.Patterns.Behavioral.Visitor.Entries;

public sealed record Vacation(int Days) : ICalendarEntry
{
    public TResult Accept<TResult>(ICalendarVisitor<TResult> visitor) => visitor.Visit(this);
}
