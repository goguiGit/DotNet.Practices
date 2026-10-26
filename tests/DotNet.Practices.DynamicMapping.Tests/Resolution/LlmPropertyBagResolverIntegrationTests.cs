using System.ClientModel;
using Azure.AI.OpenAI;
using DotNet.Practices.DynamicMapping.Models;
using DotNet.Practices.DynamicMapping.Resolution;
using DotNet.Practices.DynamicMapping.Tests.Fixtures;
using DotNet.Practices.DynamicMapping.Validators;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;

namespace DotNet.Practices.DynamicMapping.Tests.Resolution;

[Category("Integration")]
public class LlmPropertyBagResolverIntegrationTests
{
    public static IEnumerable<TestCaseData> AllBags =>
        PropertyBagFixtures.AllFileNames.SelectMany(fileName => PropertyBagFixtures.Read(fileName)
            .Where(bag => bag.Extends.Count == 0)
            .Select(bag => new TestCaseData(fileName, bag.Id).SetName($"{fileName}/{bag.Id}")));

    private static IChatClient? _chatClient;

    [OneTimeSetUp]
    public void CreateChatClient()
    {
        var configuration = new ConfigurationBuilder().AddUserSecrets<LlmPropertyBagResolverIntegrationTests>().Build();
        var endpoint = configuration["OpenAI:Endpoint"];
        var apiKey = configuration["OpenAI:ApiKey"];
        if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(apiKey))
        {
            Assert.Ignore("OpenAI:Endpoint and OpenAI:ApiKey not set in user-secrets (DotNet.Practices.Secrets).");
        }

        _chatClient = new AzureOpenAIClient(new Uri(endpoint), new ApiKeyCredential(apiKey))
            .GetChatClient(configuration["OpenAI:DeploymentName"] ?? "gpt-4o")
            .AsIChatClient();
    }

    [OneTimeTearDown]
    public void DisposeChatClient() => _chatClient?.Dispose();

    [TestCaseSource(nameof(AllBags))]
    public async Task ResolveAsync_EveryPropertyGetsAValidValue(string fileName, string bagId)
    {
        var bag = PropertyBagFixtures.Read(fileName).Single(candidate => candidate.Id == bagId);

        var result = await new LlmPropertyBagResolver(_chatClient!).ResolveAsync(bag, SampleContext.AdaLovelace);

        Log(bag, result);
        Assert.That(Violations(bag, result), Is.Empty);
    }

    private static IEnumerable<string> Violations(PropertyBag bag, ResolvedBag result) =>
        from property in bag.Properties
        let value = result.Values.GetValueOrDefault(property.Id)
        let errors = result.Values.ContainsKey(property.Id) ? PropertyValidator.Validate(property, value) : ["missing"]
        from error in errors
        select $"{property.Id}: {error}";

    private static void Log(PropertyBag bag, ResolvedBag result)
    {
        TestContext.Out.WriteLine(bag.Id);
        foreach (var property in bag.Properties)
        {
            TestContext.Out.WriteLine($"  {property.Id,-30} = {result.Values.GetValueOrDefault(property.Id) ?? "(null)"}");
        }
    }
}
