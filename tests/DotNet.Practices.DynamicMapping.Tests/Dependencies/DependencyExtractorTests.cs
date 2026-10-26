using DotNet.Practices.DynamicMapping.Dependencies;

namespace DotNet.Practices.DynamicMapping.Tests.Dependencies;

public class DependencyExtractorTests
{
    [TestCase("$Context.SamAccountName", "SamAccountName")]
    [TestCase("$Services.GetIdToCheck(Context.Domain.Key,Context.SamAccountName)", "Domain", "SamAccountName")]
    [TestCase("$ $'CN={Context.SamAccountName},{Services.GetProvisioningContainer(Context.Options.ProvisioningMapping.Values, 'X')}'", "Options", "SamAccountName")]
    [TestCase("$Services.GetFiveTwoOneIdValue(Context.Owner.Lastname, Context.Owner.Firstname, false, '26')", "Owner")]
    [TestCase("$(Context.SharedFolderParent == null ? Context.ShareRoot.Key : Context.Relations?.SharedFolderParent?.Path) + Context.Name", "Name", "Relations", "ShareRoot", "SharedFolderParent")]
    [TestCase("$$'DD_QAACC26_{Context.Name}'", "Name")]
    public void Extract_ReturnsFirstSegmentAfterContextOnce(string expression, params string[] expected)
    {
        Assert.That(DependencyExtractor.Extract(expression), Is.EquivalentTo(expected));
    }

    [TestCase("qaacc.int")]
    [TestCase("false")]
    [TestCase("sf-read-access")]
    [TestCase("")]
    [TestCase(null)]
    public void Extract_LiteralOrEmpty_ReturnsNothing(string? expression)
    {
        Assert.That(DependencyExtractor.Extract(expression), Is.Empty);
    }

    [Test]
    public void Extract_ContextWordInsideAnotherIdentifier_IsIgnored()
    {
        Assert.That(DependencyExtractor.Extract("$MyContext.Foo + Context.Bar"), Is.EquivalentTo(new[] { "Bar" }));
    }
}
