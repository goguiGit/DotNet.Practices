namespace DotNet.Practices.Abstractions.Military;

public class Soldier(string name, Rank rank)
{
    private readonly List<Soldier> _subordinates = [];
    private readonly List<Order> _receivedOrders = [];

    public string Name { get; } = name;

    public Rank Rank { get; } = rank;

    public bool CanCommand => Rank > Rank.Private;

    public IReadOnlyList<Soldier> Subordinates => _subordinates;

    public IReadOnlyList<Order> ReceivedOrders => _receivedOrders;

    public void AddSubordinate(Soldier subordinate)
    {
        ArgumentNullException.ThrowIfNull(subordinate);
        EnsureCanCommand();

        if (subordinate.Rank >= Rank)
        {
            throw new ArgumentException(
                $"{subordinate.Name} ({subordinate.Rank}) cannot serve under {Name} ({Rank}).",
                nameof(subordinate));
        }

        _subordinates.Add(subordinate);
    }

    public void GiveOrder(Order order)
    {
        EnsureCanCommand();

        foreach (var subordinate in _subordinates)
        {
            subordinate.Receive(order);
        }
    }

    private void Receive(Order order)
    {
        _receivedOrders.Add(order);

        foreach (var subordinate in _subordinates)
        {
            subordinate.Receive(order);
        }
    }

    private void EnsureCanCommand()
    {
        if (!CanCommand)
        {
            throw new InvalidOperationException($"{Name} ({Rank}) cannot command.");
        }
    }
}
