using DotNet.Practices.DynamicMapping.Models;
using DotNet.Practices.DynamicMapping.Validators;

namespace DotNet.Practices.DynamicMapping.Tests.Validators;

public class PropertyValidatorTests
{
    private static PropertyDefinition Text(PropertyValidators validators) => new("P", PropertyType.Text) { Validators = validators };

    private static readonly PropertyDefinition FutureEndDate = new("EndDate", PropertyType.Date) { Validators = new PropertyValidators { FutureDate = true } };

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void Validate_RequiredWithoutValue_ReportsRequired(string? value)
    {
        Assert.That(PropertyValidator.Validate(Text(new PropertyValidators { Required = true }), value), Is.EqualTo(new[] { "required" }));
    }

    [TestCase(null)]
    [TestCase("")]
    public void Validate_OptionalWithoutValue_IsValid(string? value)
    {
        Assert.That(PropertyValidator.Validate(Text(new PropertyValidators { MaxLength = 3, Pattern = "^a+$" }), value), Is.Empty);
    }

    [Test]
    public void Validate_LongerThanMaxLength_ReportsMaxLength()
    {
        Assert.That(PropertyValidator.Validate(Text(new PropertyValidators { MaxLength = 3 }), "abcd"), Is.EqualTo(new[] { "maxlength 3" }));
    }

    [Test]
    public void Validate_NotMatchingPattern_ReportsPattern()
    {
        Assert.That(PropertyValidator.Validate(Text(new PropertyValidators { Pattern = @"^[^@]+$" }), "a@b"), Is.EqualTo(new[] { "pattern" }));
    }

    [Test]
    public void Validate_PastDateWhenFutureRequired_ReportsFutureDate()
    {
        Assert.That(PropertyValidator.Validate(FutureEndDate, "2000-01-01"), Is.EqualTo(new[] { "futureDate" }));
    }

    [Test]
    public void Validate_TodayWhenFutureRequired_ReportsFutureDate()
    {
        Assert.That(PropertyValidator.Validate(FutureEndDate, DateTime.UtcNow.ToString("yyyy-MM-dd")), Is.EqualTo(new[] { "futureDate" }));
    }

    [Test]
    public void Validate_FutureDate_IsValid()
    {
        Assert.That(PropertyValidator.Validate(FutureEndDate, DateTime.UtcNow.AddYears(1).ToString("yyyy-MM-dd")), Is.Empty);
    }

    [TestCase(PropertyType.Date, "not a date", "date")]
    [TestCase(PropertyType.Date, "2030", "date")]
    [TestCase(PropertyType.Date, "17/05/2030", "date")]
    [TestCase(PropertyType.Boolean, "yes", "boolean")]
    [TestCase(PropertyType.Integer, "1.5", "integer")]
    [TestCase(PropertyType.UniqueIdentifier, "123", "uniqueIdentifier")]
    public void Validate_ValueNotMatchingType_ReportsType(PropertyType type, string value, string expected)
    {
        Assert.That(PropertyValidator.Validate(new PropertyDefinition("P", type), value), Is.EqualTo(new[] { expected }));
    }

    [TestCase(PropertyType.Date, "2030-05-17")]
    [TestCase(PropertyType.Date, "2030-05-17T10:00:00Z")]
    [TestCase(PropertyType.Boolean, "true")]
    [TestCase(PropertyType.Integer, "42")]
    [TestCase(PropertyType.UniqueIdentifier, "6f9619ff-8b86-d011-b42d-00c04fc964ff")]
    [TestCase(PropertyType.Text, "anything")]
    [TestCase(PropertyType.Lookup, "qaacc.int")]
    [TestCase(null, "inherited type is not checked")]
    public void Validate_ValueMatchingType_IsValid(PropertyType? type, string value)
    {
        Assert.That(PropertyValidator.Validate(new PropertyDefinition("P", type), value), Is.Empty);
    }

    [Test]
    public void Validate_SeveralViolations_ReportsAllInOrder()
    {
        var property = Text(new PropertyValidators { Required = true, MaxLength = 2, Pattern = "^a+$" });

        Assert.That(PropertyValidator.Validate(property, "bbb"), Is.EqualTo(new[] { "pattern", "maxlength 2" }));
    }

    [Test]
    public void Validate_PatternThatTimesOut_ReportsPatternInsteadOfHanging()
    {
        // Patterns come from external JSON; a catastrophic one must not stall the validator.
        var property = Text(new PropertyValidators { Pattern = "^(a|a?)+$" });

        Assert.That(PropertyValidator.Validate(property, new string('a', 60) + "!"), Is.EqualTo(new[] { "pattern" }));
    }
}
