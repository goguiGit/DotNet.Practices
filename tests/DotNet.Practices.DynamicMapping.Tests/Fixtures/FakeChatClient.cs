using Microsoft.Extensions.AI;

namespace DotNet.Practices.DynamicMapping.Tests.Fixtures;

internal sealed class FakeChatClient(string responseText) : IChatClient
{
    public IReadOnlyList<ChatMessage>? LastMessages { get; private set; }

    public ChatOptions? LastOptions { get; private set; }

    public string LastPrompt => string.Join("\n", LastMessages?.Select(message => message.Text) ?? []);

    public Task<ChatResponse> GetResponseAsync(IEnumerable<ChatMessage> messages, ChatOptions? options = null, CancellationToken cancellationToken = default)
    {
        LastMessages = messages.ToList();
        LastOptions = options;
        return Task.FromResult(new ChatResponse(new ChatMessage(ChatRole.Assistant, responseText)));
    }

    public IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(IEnumerable<ChatMessage> messages, ChatOptions? options = null, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public object? GetService(Type serviceType, object? serviceKey = null) => null;

    public void Dispose()
    {
    }
}
