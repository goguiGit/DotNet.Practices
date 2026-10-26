using DotNet.Practices.DynamicMapping.Dependencies;
using DotNet.Practices.DynamicMapping.Models;
using DotNet.Practices.DynamicMapping.Tests.Fixtures;

namespace DotNet.Practices.DynamicMapping.Tests.Dependencies;

public class DependencyGraphTests
{
    private static readonly DependencyGraph AdminAdAccount = DependencyGraph.Build(PropertyBagFixtures.Read("AdminAdAccount.propertybags.json").Single());

    private static PropertyBag Bag(params PropertyDefinition[] properties) => new("Bag", properties);

    private static PropertyDefinition Text(string id, string? valueGenerator = null, string? defaultValue = null) =>
        new(id, PropertyType.Text) { ValueGenerator = valueGenerator, DefaultValue = defaultValue };

    [TestCase("DisplayName", "SamAccountName")]
    [TestCase("DistinguishedName", "SamAccountName")]
    [TestCase("IdForTargetSystem", "Domain", "SamAccountName")]
    [TestCase("SamAccountName")]
    [TestCase("Comments")]
    public void DependenciesOf_AdminAdAccount(string propertyId, params string[] expected)
    {
        Assert.That(AdminAdAccount.DependenciesOf(propertyId), Is.EquivalentTo(expected));
    }

    [TestCase("SamAccountName", "Owner")]
    [TestCase("DistinguishedName", "Options")]
    [TestCase("DisplayName")]
    public void ExternalReferencesOf_AdminAdAccount(string propertyId, params string[] expected)
    {
        Assert.That(AdminAdAccount.ExternalReferencesOf(propertyId), Is.EquivalentTo(expected));
    }

    [Test]
    public void EvaluationOrder_AdminAdAccount_KeepsDeclarationOrderBecauseDependenciesAlreadyComeFirst()
    {
        Assert.That(AdminAdAccount.EvaluationOrder(), Is.EqualTo(new[]
        {
            "Domain", "SamAccountName", "DisplayName", "AdGuid", "DistinguishedName", "IdForTargetSystem",
            "UPNOverride", "StartDate", "EndDate", "Comments", "Description",
        }));
    }

    [Test]
    public void EvaluationOrder_WithoutDependencies_KeepsDeclarationOrder()
    {
        Assert.That(DependencyGraph.Build(Bag(Text("C"), Text("A"), Text("B"))).EvaluationOrder(), Is.EqualTo(new[] { "C", "A", "B" }));
    }

    [Test]
    public void EvaluationOrder_MovesAPropertyAfterDependenciesDeclaredLater()
    {
        var graph = DependencyGraph.Build(Bag(Text("Full", "$Context.First + Context.Last"), Text("First"), Text("Last")));

        Assert.That(graph.EvaluationOrder(), Is.EqualTo(new[] { "First", "Last", "Full" }));
    }

    [Test]
    public void EvaluationOrder_UsesDefaultValueAndIdentifyingGeneratorToo()
    {
        var graph = DependencyGraph.Build(Bag(
            Text("C", defaultValue: "$Context.B"),
            new PropertyDefinition("B", PropertyType.Text) { IdentifyingGenerator = "$Services.Next(Context.A)" },
            Text("A")));

        Assert.That(graph.EvaluationOrder(), Is.EqualTo(new[] { "A", "B", "C" }));
    }

    [Test]
    public void Build_ReferenceToPropertyOutsideTheBag_IsExternal()
    {
        var readAccess = DependencyGraph.Build(PropertyBagFixtures.Read("SharedFolder.propertybags.json")[1]);

        Assert.That(readAccess.ExternalReferencesOf("GroupName"), Is.EquivalentTo(new[] { "ResourceSequenceId" }));
    }

    [Test]
    public void EvaluationOrder_WithCycle_ThrowsNamingTheProperties()
    {
        var graph = DependencyGraph.Build(Bag(Text("A", "$Context.B"), Text("B", "$Context.A"), Text("C")));

        Assert.That(() => graph.EvaluationOrder(), Throws.TypeOf<CyclicDependencyException>().With.Message.Contains("A, B"));
    }

    [Test]
    public void EvaluationOrder_SelfReference_IsACycle()
    {
        Assert.That(() => DependencyGraph.Build(Bag(Text("A", "$Context.A"))).EvaluationOrder(), Throws.TypeOf<CyclicDependencyException>());
    }

    [Test]
    public void DependenciesOf_UnknownProperty_ThrowsKeyNotFound()
    {
        Assert.That(() => DependencyGraph.Build(Bag(Text("A"))).DependenciesOf("Nope"), Throws.TypeOf<KeyNotFoundException>());
    }
}
