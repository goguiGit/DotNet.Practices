using DotNet.Practices.Abstractions.Translation.Interfaces;
using Microsoft.Extensions.AI;

namespace DotNet.Practices.Abstractions.Translation;

public class TranslationEngine(IChatClient chatClient, ILogger<TranslationEngine> logger) : ITranslationEngine
{
    private const string SystemInstruction =
        "You are an assistant that translates texts of a human resources application, preserving HTML structure. " +
        "Just translate the text without comments and if you're not sure answer blank.";

    private readonly IChatClient _chatClient = chatClient ?? throw new ArgumentNullException(nameof(chatClient));
    private readonly ILogger<TranslationEngine> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task<string> TranslateAsync(Language fromLanguage, Language toLanguage, string text, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }

        try
        {
            List<ChatMessage> messages =
            [
                new(ChatRole.System, SystemInstruction),
                new(ChatRole.User, $"Translate this from {fromLanguage} to {toLanguage}: {text}"),
            ];

            var response = await _chatClient.GetResponseAsync(messages, cancellationToken: cancellationToken);
            return response.Text.TrimStart('\r', '\n').TrimEnd('.');
        }
        catch (Exception exception)
        {
            // Translation is best effort: callers fall back to the original text instead of failing the request.
            _logger.LogError(exception, "Translation from {FromLanguage} to {ToLanguage} failed.", fromLanguage, toLanguage);
            return string.Empty;
        }
    }
}
