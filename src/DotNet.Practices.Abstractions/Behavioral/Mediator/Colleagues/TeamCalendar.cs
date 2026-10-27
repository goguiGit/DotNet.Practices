namespace DotNet.Practices.Abstractions.Behavioral.Mediator.Colleagues;

public sealed class TeamCalendar(int capacityPerDay)
{
    private readonly Dictionary<DateOnly, List<string>> _off = [];

    public IReadOnlyList<string> WhoIsOff(DateOnly day) => _off.GetValueOrDefault(day, []);

    public bool IsFull(DateOnly day) => WhoIsOff(day).Count >= capacityPerDay;

    public void Block(DateOnly day, string employee)
    {
        if (!_off.TryGetValue(day, out var names))
        {
            names = [];
            _off[day] = names;
        }

        names.Add(employee);
    }
}
