namespace DotNet.Practices.Abstractions.Behavioral.Visitor.Interfaces;

public interface ICalendarEntry
{
    TResult Accept<TResult>(ICalendarVisitor<TResult> visitor);
}
