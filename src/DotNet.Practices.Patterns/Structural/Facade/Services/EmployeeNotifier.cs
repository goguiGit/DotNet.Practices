namespace DotNet.Practices.Patterns.Structural.Facade.Services;

public sealed class EmployeeNotifier
{
    private readonly List<string> _sent = [];

    public IReadOnlyList<string> Sent => _sent;

    public void Notify(string employee, string message) => _sent.Add($"{employee}: {message}");
}
