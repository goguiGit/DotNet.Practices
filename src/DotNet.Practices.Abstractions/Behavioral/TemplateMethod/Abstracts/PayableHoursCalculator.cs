namespace DotNet.Practices.Abstractions.Behavioral.TemplateMethod.Abstracts;

public abstract class PayableHoursCalculator
{
    // Template method: the steps and their order are fixed here; subclasses fill in the rules.
    public decimal Calculate(IEnumerable<WorkedDay> days)
    {
        var payable = 0m;
        foreach (var day in days.Where(Counts))
        {
            var regular = Math.Min(day.Hours, DailyLimit);
            var overtime = day.Hours - regular;
            payable += regular + overtime * OvertimeRate;
        }

        return payable;
    }

    protected abstract decimal DailyLimit { get; }

    protected virtual decimal OvertimeRate => 1.5m;

    // Hook: counts every day unless an agreement says otherwise.
    protected virtual bool Counts(WorkedDay day) => true;
}
