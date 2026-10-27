using DotNet.Practices.Patterns.Behavioral.Strategy.Interfaces;

namespace DotNet.Practices.Patterns.Behavioral.Strategy.Notifiers;

// shortcut: records in memory instead of calling a push provider, replace when wiring real infrastructure
public class PushNotifier : INotifier
{
    private readonly List<Notification> _sent = [];

    public NotificationChannel Channel => NotificationChannel.Push;

    public IReadOnlyList<Notification> Sent => _sent;

    public Task SendAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        _sent.Add(notification);
        return Task.CompletedTask;
    }
}
