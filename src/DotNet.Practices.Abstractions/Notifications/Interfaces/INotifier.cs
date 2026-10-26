namespace DotNet.Practices.Abstractions.Notifications.Interfaces;

public interface INotifier
{
    NotificationChannel Channel { get; }

    Task SendAsync(Notification notification, CancellationToken cancellationToken = default);
}
