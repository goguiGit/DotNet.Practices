using DotNet.Practices.Patterns.Behavioral.Mediator.Interfaces;

namespace DotNet.Practices.Patterns.Behavioral.Mediator.Colleagues;

public sealed class Employee(string name, ILeaveDesk desk)
{
    private readonly List<string> _inbox = [];

    public string Name { get; } = name;

    public IReadOnlyList<string> Inbox => _inbox;

    public void RequestLeave(DateOnly day) => desk.Request(this, day);

    public void Receive(string message) => _inbox.Add(message);
}
