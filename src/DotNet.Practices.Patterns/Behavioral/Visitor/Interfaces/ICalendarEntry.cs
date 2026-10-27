namespace DotNet.Practices.Patterns.Behavioral.Visitor.Interfaces;

public interface ICalendarEntry
{
    TResult Accept<TResult>(ICalendarVisitor<TResult> visitor);
}
