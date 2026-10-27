using DotNet.Practices.Abstractions.Behavioral.TemplateMethod.Abstracts;

namespace DotNet.Practices.Abstractions.Behavioral.TemplateMethod.Agreements;

public sealed class StandardAgreement : PayableHoursCalculator
{
    protected override decimal DailyLimit => 8m;
}
