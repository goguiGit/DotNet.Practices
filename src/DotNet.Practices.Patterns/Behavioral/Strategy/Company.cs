namespace DotNet.Practices.Patterns.Behavioral.Strategy;

public record Company(string Name, IReadOnlyCollection<NotificationChannel> Channels);
