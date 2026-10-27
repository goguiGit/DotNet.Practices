using DotNet.Practices.Patterns.Behavioral.Command.Interfaces;

namespace DotNet.Practices.Patterns.Behavioral.Command;

// Invoker: runs commands and keeps them so they can be undone in reverse order.
public sealed class CommandHistory
{
    private readonly Stack<ICommand> _executed = new();

    public void Execute(ICommand command)
    {
        command.Execute();
        _executed.Push(command);
    }

    public void Undo()
    {
        if (_executed.TryPop(out var command))
        {
            command.Undo();
        }
    }
}
