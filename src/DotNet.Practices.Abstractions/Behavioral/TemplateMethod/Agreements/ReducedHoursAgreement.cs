using DotNet.Practices.Abstractions.Behavioral.TemplateMethod.Abstracts;

namespace DotNet.Practices.Abstractions.Behavioral.TemplateMethod.Agreements;

public sealed class ReducedHoursAgreement : PayableHoursCalculator
{
    protected override decimal DailyLimit => 6m;

    protected override decimal OvertimeRate => 1.25m;

    protected override bool Counts(WorkedDay day) =>
        day.Date.DayOfWeek is not (DayOfWeek.Saturday or DayOfWeek.Sunday);
}
