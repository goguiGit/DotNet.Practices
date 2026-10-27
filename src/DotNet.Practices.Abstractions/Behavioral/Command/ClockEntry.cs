namespace DotNet.Practices.Abstractions.Behavioral.Command;

public sealed record ClockEntry(TimeOnly At, ClockDirection Direction);
