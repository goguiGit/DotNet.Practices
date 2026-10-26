using DotNet.Practices.DynamicMapping.Plans;

namespace DotNet.Practices.DynamicMapping.Tests.Plans;

public class MappingPlanTests
{
    private static readonly PlanKey Key = new(new string('a', 64));

    private static readonly PropertyMapping Expression = new("DisplayName", MappingKind.Expression, "$Context.SamAccountName", ["SamAccountName"]);

    private static readonly PropertyMapping Dynamic = new("Description", MappingKind.Dynamic);

    [Test]
    public void HasDynamicProperties_WhenAllDeterministic_IsFalse()
    {
        Assert.That(new MappingPlan(Key, "v1", [Expression]).HasDynamicProperties, Is.False);
    }

    [Test]
    public void HasDynamicProperties_WhenAnyDynamic_IsTrue()
    {
        Assert.That(new MappingPlan(Key, "v1", [Expression, Dynamic]).HasDynamicProperties, Is.True);
    }

    [Test]
    public void Constructor_WithDuplicatePropertyIds_ThrowsArgumentException()
    {
        Assert.That(() => new MappingPlan(Key, "v1", [Expression, Expression]), Throws.ArgumentException);
    }

    [Test]
    public void PropertyMapping_ExpressionKindWithoutExpression_ThrowsArgumentException()
    {
        Assert.That(() => new PropertyMapping("X", MappingKind.Expression), Throws.ArgumentException);
    }

    [Test]
    public void PropertyMapping_WithExpression_KeepsItWhenCloned()
    {
        // `with` copies fields without re-running the validation; Kind and Expression are get-only so a clone cannot break the invariant.
        Assert.That((Expression with { DependsOn = [] }).Expression, Is.EqualTo("$Context.SamAccountName"));
    }
}
