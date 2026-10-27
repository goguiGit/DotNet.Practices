using DotNet.Practices.Patterns.Behavioral.Command;
using DotNet.Practices.Patterns.Behavioral.Command.Commands;

namespace DotNet.Practices.Patterns.Tests.Behavioral.Command;

public class CommandHistoryTests
{
    private static readonly ClockEntry NineIn = new(new TimeOnly(9, 0), ClockDirection.In);
    private static readonly ClockEntry FiveOut = new(new TimeOnly(17, 0), ClockDirection.Out);

    [Test]
    public void ClockCommand_Execute_AddsTheEntryToTheTimesheet()
    {
        var timesheet = new Timesheet();
        var history = new CommandHistory();

        history.Execute(new ClockCommand(timesheet, NineIn));

        Assert.That(timesheet.Entries, Is.EqualTo(new[] { NineIn }));
    }

    [Test]
    public void CommandHistory_Undo_RemovesTheLastClockEntry()
    {
        var timesheet = new Timesheet();
        var history = new CommandHistory();
        history.Execute(new ClockCommand(timesheet, NineIn));
        history.Execute(new ClockCommand(timesheet, FiveOut));

        history.Undo();

        Assert.That(timesheet.Entries, Is.EqualTo(new[] { NineIn }));
    }

    [Test]
    public void CommandHistory_Undo_RestoresTheEntryACorrectionReplaced()
    {
        var timesheet = new Timesheet();
        var history = new CommandHistory();
        history.Execute(new ClockCommand(timesheet, NineIn));
        history.Execute(new CorrectEntryCommand(timesheet, NineIn, new ClockEntry(new TimeOnly(8, 45), ClockDirection.In)));

        history.Undo();

        Assert.That(timesheet.Entries, Is.EqualTo(new[] { NineIn }));
    }

    [Test]
    public void CommandHistory_Undo_WithNothingToUndo_LeavesTheTimesheetUntouched()
    {
        var timesheet = new Timesheet();
        var history = new CommandHistory();

        history.Undo();

        Assert.That(timesheet.Entries, Is.Empty);
    }
}
