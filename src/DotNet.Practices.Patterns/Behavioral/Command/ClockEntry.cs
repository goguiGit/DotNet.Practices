namespace DotNet.Practices.Patterns.Behavioral.Command;

public sealed record ClockEntry(TimeOnly At, ClockDirection Direction);
