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
        var plan = new MappingPlan(Key, "v1", [Expression]);

        Assert.That(plan.HasDynamicProperties, Is.False);
    }

    [Test]
    public void HasDynamicProperties_WhenAnyDynamic_IsTrue()
    {
        var plan = new MappingPlan(Key, "v1", [Expression, Dynamic]);

        Assert.That(plan.HasDynamicProperties, Is.True);
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
    public void PlanKey_WithInvalidValue_ThrowsArgumentException()
    {
        Assert.That(() => new PlanKey("not-a-hash"), Throws.ArgumentException);
    }
}
