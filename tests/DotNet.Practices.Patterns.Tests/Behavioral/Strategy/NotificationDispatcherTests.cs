using DotNet.Practices.Patterns.Behavioral.Strategy.Notifiers;
using DotNet.Practices.Patterns.Behavioral.Strategy;

namespace DotNet.Practices.Patterns.Tests.Behavioral.Strategy;

public class NotificationDispatcherTests
{
    private static readonly Notification Welcome = new("Welcome", "Your account is ready");

    [Test]
    public async Task SendAsync_UsesEveryChannelEnabledForTheCompany()
    {
        var email = new EmailNotifier();
        var push = new PushNotifier();
        var dispatcher = new NotificationDispatcher([email, push]);
        var company = new Company("Acme", [NotificationChannel.Email, NotificationChannel.Push]);

        await dispatcher.SendAsync(company, Welcome);

        Assert.Multiple(() =>
        {
            Assert.That(email.Sent, Is.EqualTo(new[] { Welcome }));
            Assert.That(push.Sent, Is.EqualTo(new[] { Welcome }));
        });
    }

    [Test]
    public async Task SendAsync_SkipsChannelsNotEnabledForTheCompany()
    {
        var email = new EmailNotifier();
        var push = new PushNotifier();
        var dispatcher = new NotificationDispatcher([email, push]);
        var company = new Company("Acme", [NotificationChannel.Email]);

        await dispatcher.SendAsync(company, Welcome);

        Assert.Multiple(() =>
        {
            Assert.That(email.Sent, Is.EqualTo(new[] { Welcome }));
            Assert.That(push.Sent, Is.Empty);
        });
    }

    [Test]
    public async Task SendAsync_WhenCompanyHasNoChannels_SendsNothing()
    {
        var email = new EmailNotifier();
        var dispatcher = new NotificationDispatcher([email]);
        var company = new Company("Acme", []);

        await dispatcher.SendAsync(company, Welcome);

        Assert.That(email.Sent, Is.Empty);
    }

    [Test]
    public void SendAsync_WhenEnabledChannelHasNoNotifier_ThrowsInvalidOperationException()
    {
        var dispatcher = new NotificationDispatcher([new EmailNotifier()]);
        var company = new Company("Acme", [NotificationChannel.Push]);

        Assert.That(() => dispatcher.SendAsync(company, Welcome), Throws.InvalidOperationException);
    }

    [Test]
    public void Constructor_WithDuplicateChannel_ThrowsArgumentException()
    {
        Assert.That(
            () => new NotificationDispatcher([new EmailNotifier(), new EmailNotifier()]),
            Throws.ArgumentException);
    }
}
