using DotNet.Practices.Abstractions.Notifications.Interfaces;

namespace DotNet.Practices.Abstractions.Notifications;

public class NotificationDispatcher
{
    private readonly IReadOnlyDictionary<NotificationChannel, INotifier> _notifiers;

    public NotificationDispatcher(IEnumerable<INotifier> notifiers)
    {
        ArgumentNullException.ThrowIfNull(notifiers);

        try
        {
            _notifiers = notifiers.ToDictionary(notifier => notifier.Channel);
        }
        catch (ArgumentException exception)
        {
            throw new ArgumentException("Only one notifier per channel is allowed.", nameof(notifiers), exception);
        }
    }

    public Task SendAsync(Company company, Notification notification, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(company);
        ArgumentNullException.ThrowIfNull(notification);

        var sends = company.Channels.Distinct().Select(channel => ResolveNotifier(channel).SendAsync(notification, cancellationToken));
        return Task.WhenAll(sends);
    }

    private INotifier ResolveNotifier(NotificationChannel channel) =>
        _notifiers.TryGetValue(channel, out var notifier)
            ? notifier
            : throw new InvalidOperationException($"No notifier registered for channel {channel}.");
}
