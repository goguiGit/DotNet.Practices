using System.Text;
using DotNet.Practices.AgentKit.Generators.Interfaces;
using DotNet.Practices.AgentKit.Models;

namespace DotNet.Practices.AgentKit.Generators;

public sealed class ClaudeCodeGenerator : IGenerator
{
    private const string WriteTools = "Edit, Write, NotebookEdit";

    public string Tool => "Claude Code";

    public IReadOnlyList<GeneratedFile> Generate(KitSource source)
    {
        ArgumentNullException.ThrowIfNull(source);

        var rules = source.Rules.Select(rule => new GeneratedFile(
            $".claude/rules/{rule.Name}.md",
            Markdown.Marker($"source/rules/{rule.Name}.md") + Markdown.NewLine + Markdown.NewLine + rule.Body));

        var agents = source.Agents.Select(agent => new GeneratedFile($".claude/agents/{agent.Name}.md", Agent(agent)));

        return [.. rules, .. agents];
    }

    private static string Agent(AgentDefinition agent)
    {
        var text = new StringBuilder();
        text.Append("---").Append(Markdown.NewLine);
        text.Append("name: ").Append(agent.Name).Append(Markdown.NewLine);
        text.Append("description: ").Append(agent.Description).Append(Markdown.NewLine);
        if (agent.Tools.Count > 0)
        {
            text.Append("tools: ").Append(string.Join(", ", agent.Tools)).Append(Markdown.NewLine);
        }

        if (agent.ReadOnly)
        {
            text.Append("disallowedTools: ").Append(WriteTools).Append(Markdown.NewLine);
        }

        text.Append("model: ").Append(agent.Model).Append(Markdown.NewLine);
        text.Append("---").Append(Markdown.NewLine);
        text.Append(Markdown.Marker($"source/agents/{agent.Name}.md")).Append(Markdown.NewLine).Append(Markdown.NewLine);
        text.Append(agent.Body);
        return text.ToString();
    }
}
