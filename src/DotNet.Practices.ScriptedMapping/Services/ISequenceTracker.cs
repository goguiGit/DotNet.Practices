namespace DotNet.Practices.ScriptedMapping.Services;

public interface ISequenceTracker
{
    int Next(string sequence, int initial = 1);
}
