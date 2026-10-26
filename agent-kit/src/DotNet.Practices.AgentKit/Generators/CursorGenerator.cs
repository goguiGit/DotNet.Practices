using System.Text;
using DotNet.Practices.AgentKit.Generators.Interfaces;
using DotNet.Practices.AgentKit.Models;

namespace DotNet.Practices.AgentKit.Generators;

public sealed class CursorGenerator : IGenerator
{
    public string Tool => "Cursor";

    public IReadOnlyList<GeneratedFile> Generate(KitSource source)
    {
        ArgumentNullException.ThrowIfNull(source);

        var rules = source.Rules.Select(rule => new GeneratedFile(
            $".cursor/rules/{rule.Name}.mdc",
            Mdc(rule.Title, alwaysApply: true, $"source/rules/{rule.Name}.md", rule.Body)));

        var roles = new StringBuilder();
        foreach (var agent in source.Agents)
        {
            roles.Append("## ").Append(agent.Name).Append(agent.ReadOnly ? " (solo lectura)" : string.Empty).Append(Markdown.NewLine).Append(Markdown.NewLine);
            roles.Append(agent.Description).Append(Markdown.NewLine).Append(Markdown.NewLine);
            roles.Append(agent.Body).Append(Markdown.NewLine);
        }

        // Cursor has no agent concept; the roles become a rule the user attaches on demand.
        var agentRoles = new GeneratedFile(".cursor/rules/agent-roles.mdc", Mdc("Roles de agente del repositorio", alwaysApply: false, "source/agents/", roles.ToString()));

        return [.. rules, agentRoles];
    }

    private static string Mdc(string description, bool alwaysApply, string sourcePath, string body) =>
        "---" + Markdown.NewLine +
        "description: " + description + Markdown.NewLine +
        "alwaysApply: " + (alwaysApply ? "true" : "false") + Markdown.NewLine +
        "---" + Markdown.NewLine +
        Markdown.Marker(sourcePath) + Markdown.NewLine + Markdown.NewLine +
        body;
}
