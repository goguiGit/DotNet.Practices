namespace DotNet.Practices.Patterns.Behavioral.Command.Interfaces;

public interface ICommand
{
    void Execute();

    void Undo();
}
