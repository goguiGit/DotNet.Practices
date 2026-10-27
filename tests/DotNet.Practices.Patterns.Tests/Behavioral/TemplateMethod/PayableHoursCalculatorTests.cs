using DotNet.Practices.Patterns.Behavioral.TemplateMethod;
using DotNet.Practices.Patterns.Behavioral.TemplateMethod.Agreements;

namespace DotNet.Practices.Patterns.Tests.Behavioral.TemplateMethod;

public class PayableHoursCalculatorTests
{
    private static readonly DateOnly Monday = new(2026, 10, 5);
    private static readonly DateOnly Tuesday = new(2026, 10, 6);
    private static readonly DateOnly Saturday = new(2026, 10, 10);

    [Test]
    public void StandardAgreement_Calculate_PaysHoursOverEightAtOneAndAHalf()
    {
        var payable = new StandardAgreement().Calculate([new WorkedDay(Monday, 8), new WorkedDay(Tuesday, 10)]);

        Assert.That(payable, Is.EqualTo(19m));
    }

    [Test]
    public void ReducedHoursAgreement_Calculate_PaysHoursOverSixAtOneAndAQuarter()
    {
        var payable = new ReducedHoursAgreement().Calculate([new WorkedDay(Monday, 8)]);

        Assert.That(payable, Is.EqualTo(8.5m));
    }

    [Test]
    public void ReducedHoursAgreement_Calculate_IgnoresWeekendDays()
    {
        var payable = new ReducedHoursAgreement().Calculate([new WorkedDay(Saturday, 4)]);

        Assert.That(payable, Is.EqualTo(0m));
    }

    [Test]
    public void StandardAgreement_Calculate_CountsWeekendDays()
    {
        var payable = new StandardAgreement().Calculate([new WorkedDay(Saturday, 4)]);

        Assert.That(payable, Is.EqualTo(4m));
    }
}
