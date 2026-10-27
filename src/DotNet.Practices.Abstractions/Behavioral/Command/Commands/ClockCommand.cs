using DotNet.Practices.Abstractions.Behavioral.Command.Interfaces;

namespace DotNet.Practices.Abstractions.Behavioral.Command.Commands;

public sealed class ClockCommand(Timesheet timesheet, ClockEntry entry) : ICommand
{
    public void Execute() => timesheet.Add(entry);

    public void Undo() => timesheet.Remove(entry);
}
