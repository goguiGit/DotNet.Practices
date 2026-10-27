using DotNet.Practices.Abstractions.Behavioral.Visitor.Interfaces;

namespace DotNet.Practices.Abstractions.Behavioral.Visitor.Entries;

public sealed record SickLeave(int Days, bool IsCertified) : ICalendarEntry
{
    public TResult Accept<TResult>(ICalendarVisitor<TResult> visitor) => visitor.Visit(this);
}
