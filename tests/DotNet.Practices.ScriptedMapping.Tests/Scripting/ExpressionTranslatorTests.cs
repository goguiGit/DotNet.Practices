using DotNet.Practices.ScriptedMapping.Scripting;

namespace DotNet.Practices.ScriptedMapping.Tests.Scripting;

public class ExpressionTranslatorTests
{
    [TestCase("$Context.SamAccountName", "Context.SamAccountName")]
    [TestCase("$ $'CN={Context.SamAccountName},OU=Admins'", " $\"CN={Context.SamAccountName},OU=Admins\"")]
    [TestCase("$$'DD_QAACC26_{Context.Name}'", "$\"DD_QAACC26_{Context.Name}\"")]
    [TestCase("$Services.GetFiveTwoOneIdValue(Context.Owner.Lastname, Context.Owner.Firstname, false, '-adm-26')", "Services.GetFiveTwoOneIdValue(Context.Owner.Lastname, Context.Owner.Firstname, false, \"-adm-26\")")]
    public void ToScript_DropsTheDollarAndTurnsApostrophesIntoQuotes(string expression, string expected)
    {
        Assert.That(ExpressionTranslator.ToScript(expression), Is.EqualTo(expected));
    }

    [TestCase("$Context.X")]
    [TestCase("$$'x'")]
    public void IsScript_WithDollarPrefix_IsTrue(string expression)
    {
        Assert.That(ExpressionTranslator.IsScript(expression), Is.True);
    }

    [TestCase("qaacc.int")]
    [TestCase("false")]
    [TestCase("")]
    [TestCase(null)]
    public void IsScript_WithoutDollarPrefix_IsFalse(string? expression)
    {
        Assert.That(ExpressionTranslator.IsScript(expression), Is.False);
    }

    [Test]
    public void ToScript_WithoutDollarPrefix_ThrowsArgumentException()
    {
        Assert.That(() => ExpressionTranslator.ToScript("qaacc.int"), Throws.ArgumentException);
    }
}
