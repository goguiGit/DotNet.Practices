using DotNet.Practices.Abstractions.Notifications.Interfaces;

namespace DotNet.Practices.Abstractions.Notifications.Notifiers;

// shortcut: records in memory instead of sending mail, replace with an SMTP client when wiring real infrastructure
public class EmailNotifier : INotifier
{
    private readonly List<Notification> _sent = [];

    public NotificationChannel Channel => NotificationChannel.Email;

    public IReadOnlyList<Notification> Sent => _sent;

    public Task SendAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        _sent.Add(notification);
        return Task.CompletedTask;
    }
}
