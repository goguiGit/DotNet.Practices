using DotNet.Practices.AgentKit.Models;

namespace DotNet.Practices.AgentKit.Tests.Fixtures;

public static class SampleSource
{
    public static readonly RuleDocument Naming = new("naming", "# Convención de nombres\r\n\r\nTodo en inglés.\r\n");

    public static readonly RuleDocument Comments = new("comments", "# Comentarios\r\n\r\nSolo el porqué.\r\n");

    public static readonly AgentDefinition Researcher = new(
        Name: "researcher",
        Description: "Exploración de solo lectura.",
        Model: "sonnet",
        ReadOnly: true,
        Tools: ["Read", "Grep", "Glob", "Bash"],
        Body: "Investigas y respondes.\r\n");

    public static readonly AgentDefinition Implementer = new(
        Name: "implementer",
        Description: "Implementa una tarea acotada.",
        Model: "sonnet",
        ReadOnly: false,
        Tools: ["Read", "Grep", "Glob", "Bash", "Edit", "Write"],
        Body: "Implementas con TDD.\r\n");

    public static KitSource Kit => new(
        Instructions: "Lee `CLAUDE.md` primero.\r\n",
        Rules: [Naming, Comments],
        Agents: [Researcher, Implementer]);
}
