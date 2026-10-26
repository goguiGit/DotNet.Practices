using System.Text;
using DotNet.Practices.AgentKit.Generators.Interfaces;
using DotNet.Practices.AgentKit.Models;

namespace DotNet.Practices.AgentKit.Generators;

// AGENTS.md is the convention Codex and several other tools read from the repository root.
public sealed class AgentsMdGenerator : IGenerator
{
    public string Tool => "AGENTS.md (Codex and others)";

    public IReadOnlyList<GeneratedFile> Generate(KitSource source)
    {
        ArgumentNullException.ThrowIfNull(source);

        var text = new StringBuilder();
        text.Append(Markdown.Marker("source/")).Append(Markdown.NewLine).Append(Markdown.NewLine);
        text.Append("# Instrucciones para agentes").Append(Markdown.NewLine).Append(Markdown.NewLine);
        if (source.Instructions.Length > 0)
        {
            text.Append(source.Instructions).Append(Markdown.NewLine);
        }

        text.Append("## Reglas").Append(Markdown.NewLine).Append(Markdown.NewLine);
        foreach (var rule in source.Rules)
        {
            text.Append(Markdown.DemoteHeadings(rule.Body, 2)).Append(Markdown.NewLine);
        }

        text.Append("## Roles").Append(Markdown.NewLine).Append(Markdown.NewLine);
        text.Append("Estas herramientas no tienen agentes separados; adopta el rol que pida la tarea y respeta sus límites.").Append(Markdown.NewLine).Append(Markdown.NewLine);
        foreach (var agent in source.Agents)
        {
            text.Append("### ").Append(agent.Name).Append(agent.ReadOnly ? " (solo lectura)" : string.Empty).Append(Markdown.NewLine).Append(Markdown.NewLine);
            text.Append(agent.Description).Append(Markdown.NewLine).Append(Markdown.NewLine);
            text.Append(agent.Body).Append(Markdown.NewLine);
        }

        return [new GeneratedFile("AGENTS.md", text.ToString())];
    }
}
