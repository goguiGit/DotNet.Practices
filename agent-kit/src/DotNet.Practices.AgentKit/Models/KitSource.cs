namespace DotNet.Practices.AgentKit.Models;

public sealed record KitSource(string Instructions, IReadOnlyList<RuleDocument> Rules, IReadOnlyList<AgentDefinition> Agents);
