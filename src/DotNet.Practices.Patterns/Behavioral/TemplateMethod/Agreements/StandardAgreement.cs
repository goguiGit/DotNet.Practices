using DotNet.Practices.Patterns.Behavioral.TemplateMethod.Abstracts;

namespace DotNet.Practices.Patterns.Behavioral.TemplateMethod.Agreements;

public sealed class StandardAgreement : PayableHoursCalculator
{
    protected override decimal DailyLimit => 8m;
}
