namespace DotNet.Practices.Abstractions.Behavioral.Command.Interfaces;

public interface ICommand
{
    void Execute();

    void Undo();
}
