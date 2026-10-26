using DotNet.Practices.Abstractions.Translation;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging.Abstractions;

namespace DotNet.Practices.Abstractions.Tests.Translation;

public class TranslationEngineTests
{
    private static TranslationEngine CreateEngine(FakeChatClient client) =>
        new(client, NullLogger<TranslationEngine>.Instance);

    [Test]
    public async Task TranslateAsync_WithBlankText_ReturnsEmptyWithoutCallingClient()
    {
        var client = new FakeChatClient("Hola");
        var engine = CreateEngine(client);

        var result = await engine.TranslateAsync(Language.English, Language.Spanish, "  ");

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Empty);
            Assert.That(client.LastMessages, Is.Null);
        });
    }

    [Test]
    public async Task TranslateAsync_ReturnsResponseWithoutLeadingLineBreaksOrTrailingPeriod()
    {
        var engine = CreateEngine(new FakeChatClient("\r\nHola."));

        var result = await engine.TranslateAsync(Language.English, Language.Spanish, "Hello");

        Assert.That(result, Is.EqualTo("Hola"));
    }

    [Test]
    public async Task TranslateAsync_SendsSystemInstructionAndUserRequest()
    {
        var client = new FakeChatClient("Hola");
        var engine = CreateEngine(client);

        await engine.TranslateAsync(Language.English, Language.Spanish, "Hello");

        Assert.Multiple(() =>
        {
            Assert.That(client.LastMessages, Has.Count.EqualTo(2));
            Assert.That(client.LastMessages![0].Role, Is.EqualTo(ChatRole.System));
            Assert.That(client.LastMessages[1].Role, Is.EqualTo(ChatRole.User));
            Assert.That(client.LastMessages[1].Text, Does.Contain("from English to Spanish: Hello"));
        });
    }

    [Test]
    public async Task TranslateAsync_WhenClientFails_ReturnsEmpty()
    {
        var engine = CreateEngine(new FakeChatClient("unused", new HttpRequestException("boom")));

        var result = await engine.TranslateAsync(Language.English, Language.Spanish, "Hello");

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void Constructor_WithNullChatClient_ThrowsArgumentNullException()
    {
        Assert.That(
            () => new TranslationEngine(null!, NullLogger<TranslationEngine>.Instance),
            Throws.ArgumentNullException);
    }
}
