namespace DotNet.Practices.AgentKit.Models;

public sealed record RuleDocument(string Name, string Body)
{
    // The first Markdown heading, or the file name when the rule has none.
    public string Title => Body.Split("\r\n").Select(line => line.TrimStart('#', ' ')).Zip(Body.Split("\r\n"))
        .Where(pair => pair.Second.StartsWith('#'))
        .Select(pair => pair.First)
        .FirstOrDefault() ?? Name;
}
