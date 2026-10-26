using DotNet.Practices.AgentKit.Models;

namespace DotNet.Practices.AgentKit.Generators.Interfaces;

public interface IGenerator
{
    string Tool { get; }

    IReadOnlyList<GeneratedFile> Generate(KitSource source);
}
