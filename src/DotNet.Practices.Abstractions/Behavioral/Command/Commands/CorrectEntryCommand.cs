using DotNet.Practices.Abstractions.Behavioral.Command.Interfaces;

namespace DotNet.Practices.Abstractions.Behavioral.Command.Commands;

public sealed class CorrectEntryCommand(Timesheet timesheet, ClockEntry existing, ClockEntry corrected) : ICommand
{
    public void Execute() => timesheet.Replace(existing, corrected);

    public void Undo() => timesheet.Replace(corrected, existing);
}
