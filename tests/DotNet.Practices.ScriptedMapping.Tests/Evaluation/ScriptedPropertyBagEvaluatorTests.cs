using DotNet.Practices.DynamicMapping.Models;
using DotNet.Practices.DynamicMapping.Schema;
using DotNet.Practices.ScriptedMapping.Evaluation;
using DotNet.Practices.ScriptedMapping.Scripting;
using DotNet.Practices.ScriptedMapping.Services;

namespace DotNet.Practices.ScriptedMapping.Tests.Evaluation;

public class ScriptedPropertyBagEvaluatorTests
{
    private static readonly string FixturesDirectory = Path.Combine(TestContext.CurrentContext.TestDirectory, "Fixtures", "PropertyBags");

    private static readonly PropertyBag AdminAdAccount = PropertyBagReader.ReadFile(Path.Combine(FixturesDirectory, "AdminAdAccount.propertybags.json")).Single();

    private static EvaluationContext AdaContext() => new(
        Owner: new Dictionary<string, object?> { ["Lastname"] = "Lovelace", ["Firstname"] = "Ada" },
        Inputs: new Dictionary<string, object?> { ["Domain"] = new LookupEntry("qaacc.int", "QAACC") },
        Options: new Dictionary<string, object?>
        {
            ["ProvisioningMapping"] = new Dictionary<string, object?>
            {
                ["Values"] = new Dictionary<string, string> { ["AdminAdAccountContainer"] = "OU=Admins,DC=qaacc,DC=int" },
            },
        });

    private static ScriptedPropertyBagEvaluator NewEvaluator() => new(new ScriptServices(new InMemorySequenceTracker()));

    private static PropertyBag Bag(params PropertyDefinition[] properties) => new("Bag", properties);

    private static PropertyDefinition Text(string id, string? valueGenerator = null, string? defaultValue = null) =>
        new(id, PropertyType.Text) { ValueGenerator = valueGenerator, DefaultValue = defaultValue };

    [Test]
    public void Evaluate_AdminAdAccount_GeneratesSamAccountNameWithTheFiveTwoOneRule()
    {
        Assert.That(NewEvaluator().Evaluate(AdminAdAccount, AdaContext())["SamAccountName"], Is.EqualTo("lovelad1-adm-26"));
    }

    [Test]
    public void Evaluate_AdminAdAccount_DisplayNameCopiesSamAccountName()
    {
        var values = NewEvaluator().Evaluate(AdminAdAccount, AdaContext());

        Assert.That(values["DisplayName"], Is.EqualTo(values["SamAccountName"]));
    }

    [Test]
    public void Evaluate_AdminAdAccount_DistinguishedNameInterpolatesSamAccountNameAndContainer()
    {
        Assert.That(NewEvaluator().Evaluate(AdminAdAccount, AdaContext())["DistinguishedName"], Is.EqualTo("CN=lovelad1-adm-26,OU=Admins,DC=qaacc,DC=int"));
    }

    [Test]
    public void Evaluate_AdminAdAccount_IdForTargetSystemUsesTheLookupKey()
    {
        Assert.That(NewEvaluator().Evaluate(AdminAdAccount, AdaContext())["IdForTargetSystem"], Is.EqualTo(@"qaacc.int\lovelad1-adm-26"));
    }

    [Test]
    public void Evaluate_AdminAdAccount_InputsComeFromTheContextAndFreeTextStaysNull()
    {
        var values = NewEvaluator().Evaluate(AdminAdAccount, AdaContext());

        Assert.That((values["Domain"], values["Comments"]), Is.EqualTo((new LookupEntry("qaacc.int", "QAACC"), (object?)null)));
    }

    [Test]
    public void Evaluate_Twice_IsDeterministic()
    {
        var first = NewEvaluator().Evaluate(AdminAdAccount, AdaContext());
        var second = NewEvaluator().Evaluate(AdminAdAccount, AdaContext());

        Assert.That(second, Is.EqualTo(first));
    }

    [Test]
    public void Evaluate_DependencyDeclaredLater_IsEvaluatedFirst()
    {
        var bag = Bag(Text("Full", "$Context.First + \" \" + Context.Last"), Text("First", defaultValue: "$\"Ada\""), Text("Last", defaultValue: "$\"Lovelace\""));

        Assert.That(NewEvaluator().Evaluate(bag, new EvaluationContext())["Full"], Is.EqualTo("Ada Lovelace"));
    }

    [TestCase("false", false)]
    [TestCase("123", 123)]
    [TestCase("1.5", 1.5)]
    [TestCase("plain", "plain")]
    public void Evaluate_LiteralDefault_IsParsedLikeRabbitDoes(string literal, object expected)
    {
        Assert.That(NewEvaluator().Evaluate(Bag(Text("P", defaultValue: literal)), new EvaluationContext())["P"], Is.EqualTo(expected));
    }

    [Test]
    public void Evaluate_Cycle_ThrowsCircularDependencyException()
    {
        var bag = Bag(Text("A", "$Context.B"), Text("B", "$Context.A"));

        Assert.That(() => NewEvaluator().Evaluate(bag, new EvaluationContext()), Throws.TypeOf<CircularDependencyException>());
    }

    [Test]
    public void Compile_SameBagTwice_ReusesTheCompiledScripts()
    {
        var evaluator = NewEvaluator();

        var first = evaluator.Compile(AdminAdAccount);
        var second = evaluator.Compile(AdminAdAccount);

        Assert.That(second, Is.SameAs(first));
    }

    [Test]
    public void Compile_InvalidScript_ThrowsScriptCompilationExceptionNamingTheProperty()
    {
        var bag = Bag(Text("Broken", "$Context.Owner.Lastname +"));

        Assert.That(() => NewEvaluator().Compile(bag), Throws.TypeOf<ScriptCompilationException>().With.Message.Contains("Broken"));
    }

    [Test]
    public void Evaluate_NullInsideExpression_LeavesThePropertyUnsetLikeRabbit()
    {
        var bag = Bag(Text("Needs", "$Context.Domain.Key"));

        Assert.That(NewEvaluator().Evaluate(bag, new EvaluationContext())["Needs"], Is.Null);
    }
}
