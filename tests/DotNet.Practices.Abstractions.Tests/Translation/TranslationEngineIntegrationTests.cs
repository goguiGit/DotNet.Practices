using System.ClientModel;
using Azure.AI.OpenAI;
using DotNet.Practices.Abstractions.Translation;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;

namespace DotNet.Practices.Abstractions.Tests.Translation;

[Category("Integration")]
public class TranslationEngineIntegrationTests
{
    [Test]
    public async Task TranslateAsync_AgainstAzureOpenAI_ReturnsSpanishText()
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<TranslationEngineIntegrationTests>()
            .Build();
        var endpoint = configuration["OpenAI:Endpoint"];
        var apiKey = configuration["OpenAI:ApiKey"];
        if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(apiKey))
        {
            Assert.Ignore("OpenAI:Endpoint and OpenAI:ApiKey not set in user-secrets (DotNet.Practices.Secrets).");
        }

        var deployment = configuration["OpenAI:DeploymentName"] ?? "gpt-4o";
        using var chatClient = new AzureOpenAIClient(new Uri(endpoint), new ApiKeyCredential(apiKey))
            .GetChatClient(deployment)
            .AsIChatClient();
        var engine = new TranslationEngine(chatClient, NullLogger<TranslationEngine>.Instance);

        var result = await engine.TranslateAsync(Language.English, Language.Spanish, "Good morning");

        Assert.That(result, Is.Not.Empty.And.Not.EqualTo("Good morning"));
    }
}
