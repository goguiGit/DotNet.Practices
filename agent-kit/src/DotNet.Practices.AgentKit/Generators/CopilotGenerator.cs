using System.Text;
using DotNet.Practices.AgentKit.Generators.Interfaces;
using DotNet.Practices.AgentKit.Models;

namespace DotNet.Practices.AgentKit.Generators;

// Copilot reads one instructions file; it has neither rules nor agents, so only the rules travel.
public sealed class CopilotGenerator : IGenerator
{
    public string Tool => "GitHub Copilot";

    public IReadOnlyList<GeneratedFile> Generate(KitSource source)
    {
        ArgumentNullException.ThrowIfNull(source);

        var text = new StringBuilder();
        text.Append(Markdown.Marker("source/")).Append(Markdown.NewLine).Append(Markdown.NewLine);
        if (source.Instructions.Length > 0)
        {
            text.Append(source.Instructions).Append(Markdown.NewLine);
        }

        foreach (var rule in source.Rules)
        {
            text.Append(rule.Body).Append(Markdown.NewLine);
        }

        return [new GeneratedFile(".github/copilot-instructions.md", text.ToString())];
    }
}
