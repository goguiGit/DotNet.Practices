using DotNet.Practices.Abstractions.Behavioral.Visitor.Interfaces;

namespace DotNet.Practices.Abstractions.Behavioral.Visitor.Entries;

public sealed record Vacation(int Days) : ICalendarEntry
{
    public TResult Accept<TResult>(ICalendarVisitor<TResult> visitor) => visitor.Visit(this);
}
