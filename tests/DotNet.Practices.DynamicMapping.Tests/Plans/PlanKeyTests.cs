using DotNet.Practices.DynamicMapping.Models;
using DotNet.Practices.DynamicMapping.Plans;
using DotNet.Practices.DynamicMapping.Validators;

namespace DotNet.Practices.DynamicMapping.Tests.Plans;

public class PlanKeyTests
{
    private const string Compiler = "compiler-v1";

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

    private static PlanKey Key(PropertyBag bag, SourceSchema? source = null, string compiler = Compiler) => PlanKey.For(bag, source ?? HrSchema, compiler);

    [Test]
    public void For_SameInputs_ReturnsSameKey()
    {
        var first = Key(Bag(Domain, SamAccountName));
        var second = Key(Bag(Domain, SamAccountName));

        Assert.That(second, Is.EqualTo(first));
    }

    [Test]
    public void For_IsSha256Hex()
    {
        Assert.That(Key(Bag(Domain)).Value, Does.Match("^[0-9a-f]{64}$"));
    }

    [Test]
    public void For_KnownInputs_MatchesGoldenHash()
    {
        // The canonical form is the cache contract: changing it silently invalidates every stored plan.
        Assert.That(Key(Bag(Domain, SamAccountName)).Value, Is.EqualTo("e94c3094d143a09513e9b60185ae2ce6a30f048208a42a597bb307977387a629"));
    }

    [Test]
    public void For_PropertyOrderDoesNotMatter()
    {
        Assert.That(Key(Bag(SamAccountName, Domain)), Is.EqualTo(Key(Bag(Domain, SamAccountName))));
    }

    [Test]
    public void For_SourceColumnOrderDoesNotMatter()
    {
        var reversed = new SourceSchema("Hr.Employees", HrSchema.Columns.Reverse().ToList());

        Assert.That(Key(Bag(Domain), reversed), Is.EqualTo(Key(Bag(Domain))));
    }

    [Test]
    public void For_ExtendsOrderDoesNotMatter()
    {
        var ab = new PropertyBag("Bag", [Domain]) { Extends = ["A", "B"] };
        var ba = new PropertyBag("Bag", [Domain]) { Extends = ["B", "A"] };

        Assert.That(Key(ba), Is.EqualTo(Key(ab)));
    }

    [Test]
    public void For_ChangingAValidator_ChangesKey()
    {
        var shorter = SamAccountName with { Validators = SamAccountName.Validators with { MaxLength = 15 } };

        Assert.That(Key(Bag(shorter)), Is.Not.EqualTo(Key(Bag(SamAccountName))));
    }

    [Test]
    public void For_ChangingAnExpression_ChangesKey()
    {
        var other = SamAccountName with { DefaultValue = "$Context.Owner.Email" };

        Assert.That(Key(Bag(other)), Is.Not.EqualTo(Key(Bag(SamAccountName))));
    }

    [Test]
    public void For_ChangingReadOnly_ChangesKey()
    {
        Assert.That(Key(Bag(Domain with { ReadOnly = true })), Is.Not.EqualTo(Key(Bag(Domain))));
    }

    [Test]
    public void For_NullAndEmptyLookup_AreDifferentKeys()
    {
        Assert.That(Key(Bag(Domain with { Lookup = "" })), Is.Not.EqualTo(Key(Bag(Domain with { Lookup = null }))));
    }

    [Test]
    public void For_FieldSeparatorInsideAValue_CannotCollide()
    {
        var pipeInLookup = Domain with { Lookup = "a|b", Relation = null };
        var pipeAsSeparator = Domain with { Lookup = "a", Relation = "b" };

        Assert.That(Key(Bag(pipeInLookup)), Is.Not.EqualTo(Key(Bag(pipeAsSeparator))));
    }

    [Test]
    public void For_AddingASourceColumn_ChangesKey()
    {
        var wider = new SourceSchema("Hr.Employees", [.. HrSchema.Columns, new SourceColumn("Department", "string")]);

        Assert.That(Key(Bag(Domain), wider), Is.Not.EqualTo(Key(Bag(Domain))));
    }

    [Test]
    public void For_ChangingASourceColumnType_ChangesKey()
    {
        var retyped = new SourceSchema("Hr.Employees", [new SourceColumn("Firstname", "string"), new SourceColumn("Lastname", "text")]);

        Assert.That(Key(Bag(Domain), retyped), Is.Not.EqualTo(Key(Bag(Domain))));
    }

    [Test]
    public void For_DifferentBagId_ChangesKey()
    {
        Assert.That(Key(new PropertyBag("StandardAdAccountData", [Domain])), Is.Not.EqualTo(Key(Bag(Domain))));
    }

    [Test]
    public void For_DifferentCompilerVersion_ChangesKey()
    {
        Assert.That(Key(Bag(Domain), compiler: "compiler-v2"), Is.Not.EqualTo(Key(Bag(Domain))));
    }

    [Test]
    public void Constructor_WithInvalidValue_ThrowsArgumentException()
    {
        Assert.That(() => new PlanKey("not-a-hash"), Throws.ArgumentException);
    }
}
