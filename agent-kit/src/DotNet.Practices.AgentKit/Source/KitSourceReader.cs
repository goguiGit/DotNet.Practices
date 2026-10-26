using DotNet.Practices.AgentKit.Models;

namespace DotNet.Practices.AgentKit.Source;

public static class KitSourceReader
{
    private const string NewLine = "\r\n";

    public static KitSource Read(string sourceDirectory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceDirectory);

        var instructionsPath = Path.Combine(sourceDirectory, "instructions.md");
        var instructions = File.Exists(instructionsPath) ? ReadText(instructionsPath) : string.Empty;

        var rules = ReadMarkdownFiles(Path.Combine(sourceDirectory, "rules"))
            .Select(path => new RuleDocument(Path.GetFileNameWithoutExtension(path), ReadText(path)))
            .ToList();

        var agents = ReadMarkdownFiles(Path.Combine(sourceDirectory, "agents"))
            .Select(ReadAgent)
            .ToList();

        return new KitSource(instructions, rules, agents);
    }

    private static IEnumerable<string> ReadMarkdownFiles(string directory) =>
        Directory.Exists(directory)
            ? Directory.EnumerateFiles(directory, "*.md").OrderBy(path => path, StringComparer.Ordinal)
            : [];

    private static AgentDefinition ReadAgent(string path)
    {
        var text = ReadText(path);
        var fileName = Path.GetFileName(path);
        if (!text.StartsWith("---" + NewLine, StringComparison.Ordinal))
        {
            throw new FormatException($"Agent '{fileName}' must start with a --- frontmatter block.");
        }

        var end = text.IndexOf(NewLine + "---" + NewLine, 3, StringComparison.Ordinal);
        if (end < 0)
        {
            throw new FormatException($"Agent '{fileName}' has an unterminated frontmatter block.");
        }

        var frontmatter = text[(3 + NewLine.Length)..end]
            .Split(NewLine, StringSplitOptions.RemoveEmptyEntries)
            .Select(line => line.Split(':', 2))
            .Where(parts => parts.Length == 2)
            .ToDictionary(parts => parts[0].Trim(), parts => parts[1].Trim(), StringComparer.Ordinal);

        var body = text[(end + NewLine.Length + 3 + NewLine.Length)..].TrimStart('\r', '\n');

        return new AgentDefinition(
            Name: Required(frontmatter, "name", fileName),
            Description: Required(frontmatter, "description", fileName),
            Model: Required(frontmatter, "model", fileName),
            ReadOnly: frontmatter.TryGetValue("readOnly", out var readOnly) && bool.Parse(readOnly),
            Tools: frontmatter.TryGetValue("tools", out var tools)
                ? tools.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                : [],
            Body: body);
    }

    private static string Required(IReadOnlyDictionary<string, string> frontmatter, string key, string fileName) =>
        frontmatter.TryGetValue(key, out var value) && value.Length > 0
            ? value
            : throw new FormatException($"Agent '{fileName}' is missing '{key}' in its frontmatter.");

    // Sources may be committed with either line ending; everything downstream assumes CRLF.
    private static string ReadText(string path) => File.ReadAllText(path).ReplaceLineEndings(NewLine);
}
