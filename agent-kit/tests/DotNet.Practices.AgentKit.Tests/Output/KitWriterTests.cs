using DotNet.Practices.AgentKit.Models;
using DotNet.Practices.AgentKit.Output;

namespace DotNet.Practices.AgentKit.Tests.Output;

public class KitWriterTests
{
    private string _repo = null!;

    [SetUp]
    public void CreateRepo()
    {
        _repo = Path.Combine(Path.GetTempPath(), "agent-kit-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_repo);
    }

    [TearDown]
    public void DeleteRepo() => Directory.Delete(_repo, recursive: true);

    private static readonly GeneratedFile Rule = new(".claude/rules/naming.md", "# Naming\r\n");

    [Test]
    public void Write_CreatesDirectoriesAndFiles()
    {
        KitWriter.Write(_repo, [Rule]);

        Assert.That(File.ReadAllText(Path.Combine(_repo, ".claude", "rules", "naming.md")), Is.EqualTo("# Naming\r\n"));
    }

    [Test]
    public void Write_ReturnsOnlyThePathsThatChanged()
    {
        KitWriter.Write(_repo, [Rule]);

        Assert.That(KitWriter.Write(_repo, [Rule, new GeneratedFile("AGENTS.md", "x\r\n")]), Is.EqualTo(new[] { "AGENTS.md" }));
    }

    [Test]
    public void Check_WhenDiskMatches_ReturnsNoDrift()
    {
        KitWriter.Write(_repo, [Rule]);

        Assert.That(KitWriter.Check(_repo, [Rule]), Is.Empty);
    }

    [Test]
    public void Check_WhenFileDiffersOrIsMissing_ReportsBoth()
    {
        File.WriteAllText(Path.Combine(_repo, "AGENTS.md"), "old");

        Assert.That(KitWriter.Check(_repo, [Rule, new GeneratedFile("AGENTS.md", "new")]), Is.EquivalentTo(new[] { ".claude/rules/naming.md", "AGENTS.md" }));
    }

    [Test]
    public void Write_RejectsPathsEscapingTheRepo()
    {
        Assert.That(() => KitWriter.Write(_repo, [new GeneratedFile("../outside.md", "x")]), Throws.ArgumentException);
    }
}
