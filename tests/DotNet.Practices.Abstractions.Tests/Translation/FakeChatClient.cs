using Microsoft.Extensions.AI;

namespace DotNet.Practices.Abstractions.Tests.Translation;

internal sealed class FakeChatClient(string responseText, Exception? failure = null) : IChatClient
{
    public IReadOnlyList<ChatMessage>? LastMessages { get; private set; }

    public Task<ChatResponse> GetResponseAsync(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        LastMessages = messages.ToList();
        return failure is null
            ? Task.FromResult(new ChatResponse(new ChatMessage(ChatRole.Assistant, responseText)))
            : Task.FromException<ChatResponse>(failure);
    }

    public IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default) => throw new NotSupportedException();

    public object? GetService(Type serviceType, object? serviceKey = null) => null;

    public void Dispose()
    {
    }
}
