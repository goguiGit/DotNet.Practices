namespace DotNet.Practices.Abstractions.Notifications;

public record Company(string Name, IReadOnlyCollection<NotificationChannel> Channels);
