using DotNet.Practices.AgentKit.Models;
using DotNet.Practices.AgentKit.Source;

namespace DotNet.Practices.AgentKit.Tests.Source;

public class KitSourceReaderTests
{
    private string _root = null!;

    [SetUp]
    public void CreateSource()
    {
        _root = Path.Combine(Path.GetTempPath(), "agent-kit-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(Path.Combine(_root, "rules"));
        Directory.CreateDirectory(Path.Combine(_root, "agents"));
        File.WriteAllText(Path.Combine(_root, "instructions.md"), "Lee `CLAUDE.md` primero.\r\n");
        File.WriteAllText(Path.Combine(_root, "rules", "naming.md"), "# Convención de nombres\r\n\r\nTodo en inglés.\r\n");
        File.WriteAllText(Path.Combine(_root, "agents", "researcher.md"), """
            ---
            name: researcher
            description: Exploración de solo lectura.
            model: sonnet
            readOnly: true
            tools: Read, Grep, Glob, Bash
            ---

            Investigas y respondes.
            """.ReplaceLineEndings("\r\n") + "\r\n");
    }

    [TearDown]
    public void DeleteSource() => Directory.Delete(_root, recursive: true);

    [Test]
    public void Read_LoadsInstructions()
    {
        Assert.That(KitSourceReader.Read(_root).Instructions, Is.EqualTo("Lee `CLAUDE.md` primero.\r\n"));
    }

    [Test]
    public void Read_LoadsRulesNamedAfterTheirFile()
    {
        Assert.That(KitSourceReader.Read(_root).Rules, Is.EqualTo(new[] { new RuleDocument("naming", "# Convención de nombres\r\n\r\nTodo en inglés.\r\n") }));
    }

    [Test]
    public void Read_ParsesAgentFrontmatterAndBody()
    {
        Assert.That(KitSourceReader.Read(_root).Agents.Single(), Is.EqualTo(new AgentDefinition(
            Name: "researcher",
            Description: "Exploración de solo lectura.",
            Model: "sonnet",
            ReadOnly: true,
            Tools: ["Read", "Grep", "Glob", "Bash"],
            Body: "Investigas y respondes.\r\n")));
    }

    [Test]
    public void Read_AgentWithoutFrontmatter_ThrowsFormatExceptionNamingTheFile()
    {
        File.WriteAllText(Path.Combine(_root, "agents", "broken.md"), "No frontmatter here.");

        Assert.That(() => KitSourceReader.Read(_root), Throws.TypeOf<FormatException>().With.Message.Contains("broken.md"));
    }

    [Test]
    public void Read_AgentMissingRequiredKey_ThrowsFormatExceptionNamingTheKey()
    {
        File.WriteAllText(Path.Combine(_root, "agents", "nodesc.md"), "---\r\nname: nodesc\r\nmodel: sonnet\r\n---\r\nbody\r\n");

        Assert.That(() => KitSourceReader.Read(_root), Throws.TypeOf<FormatException>().With.Message.Contains("description"));
    }

    [Test]
    public void Read_MissingInstructions_UsesEmptyString()
    {
        File.Delete(Path.Combine(_root, "instructions.md"));

        Assert.That(KitSourceReader.Read(_root).Instructions, Is.Empty);
    }
}
