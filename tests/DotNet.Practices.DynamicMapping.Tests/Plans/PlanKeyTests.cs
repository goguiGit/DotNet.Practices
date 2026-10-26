using DotNet.Practices.DynamicMapping.Models;
using DotNet.Practices.DynamicMapping.Plans;
using DotNet.Practices.DynamicMapping.Validators;

namespace DotNet.Practices.DynamicMapping.Tests.Plans;

public class PlanKeyTests
{
    private static readonly PropertyDefinition Domain = new("Domain", PropertyType.Lookup)
    {
        Lookup = "Lookup.AdAccountDomain",
        Validators = new PropertyValidators { Required = true },
    };

    private static readonly PropertyDefinition SamAccountName = new("SamAccountName", PropertyType.Text)
    {
        DefaultValue = "$Services.GetFiveTwoOneIdValue(Context.Owner.Lastname, Context.Owner.Firstname, false, '-adm-26')",
        Validators = new PropertyValidators { Required = true, MaxLength = 20, Pattern = "^[^\\[\\],:;\\|=@\\+\\*\\?<>/\\\\]+$" },
        Unique = "ADAccountData",
    };

    private static readonly SourceSchema HrSchema = new("Hr.Employees",
    [
        new SourceColumn("Firstname", "string"),
        new SourceColumn("Lastname", "string"),
    ]);

    private static PropertyBag Bag(params PropertyDefinition[] properties) => new("AdminAdAccountData", properties);

    [Test]
    public void For_SameInputs_ReturnsSameKey()
    {
        var first = PlanKey.For(Bag(Domain, SamAccountName), HrSchema);
        var second = PlanKey.For(Bag(Domain, SamAccountName), HrSchema);

        Assert.That(second, Is.EqualTo(first));
    }

    [Test]
    public void For_IsSha256Hex()
    {
        var key = PlanKey.For(Bag(Domain), HrSchema);

        Assert.That(key.Value, Does.Match("^[0-9a-f]{64}$"));
    }

    [Test]
    public void For_PropertyOrderDoesNotMatter()
    {
        var ordered = PlanKey.For(Bag(Domain, SamAccountName), HrSchema);
        var shuffled = PlanKey.For(Bag(SamAccountName, Domain), HrSchema);

        Assert.That(shuffled, Is.EqualTo(ordered));
    }

    [Test]
    public void For_SourceColumnOrderDoesNotMatter()
    {
        var reversed = new SourceSchema("Hr.Employees", HrSchema.Columns.Reverse().ToList());

        Assert.That(PlanKey.For(Bag(Domain), reversed), Is.EqualTo(PlanKey.For(Bag(Domain), HrSchema)));
    }

    [Test]
    public void For_ChangingAValidator_ChangesKey()
    {
        var shorter = SamAccountName with { Validators = SamAccountName.Validators with { MaxLength = 15 } };

        Assert.That(PlanKey.For(Bag(shorter), HrSchema), Is.Not.EqualTo(PlanKey.For(Bag(SamAccountName), HrSchema)));
    }

    [Test]
    public void For_ChangingAnExpression_ChangesKey()
    {
        var other = SamAccountName with { DefaultValue = "$Context.Owner.Email" };

        Assert.That(PlanKey.For(Bag(other), HrSchema), Is.Not.EqualTo(PlanKey.For(Bag(SamAccountName), HrSchema)));
    }

    [Test]
    public void For_AddingASourceColumn_ChangesKey()
    {
        var wider = new SourceSchema("Hr.Employees", [.. HrSchema.Columns, new SourceColumn("Department", "string")]);

        Assert.That(PlanKey.For(Bag(Domain), wider), Is.Not.EqualTo(PlanKey.For(Bag(Domain), HrSchema)));
    }

    [Test]
    public void For_ChangingASourceColumnType_ChangesKey()
    {
        var retyped = new SourceSchema("Hr.Employees", [new SourceColumn("Firstname", "string"), new SourceColumn("Lastname", "text")]);

        Assert.That(PlanKey.For(Bag(Domain), retyped), Is.Not.EqualTo(PlanKey.For(Bag(Domain), HrSchema)));
    }

    [Test]
    public void For_DifferentBagId_ChangesKey()
    {
        var renamed = new PropertyBag("StandardAdAccountData", [Domain]);

        Assert.That(PlanKey.For(renamed, HrSchema), Is.Not.EqualTo(PlanKey.For(Bag(Domain), HrSchema)));
    }
}
