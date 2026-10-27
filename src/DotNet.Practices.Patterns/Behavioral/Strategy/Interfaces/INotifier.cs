namespace DotNet.Practices.Patterns.Behavioral.Strategy.Interfaces;

public interface INotifier
{
    NotificationChannel Channel { get; }

    Task SendAsync(Notification notification, CancellationToken cancellationToken = default);
}
