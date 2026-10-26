using System.ClientModel;
using Azure.AI.OpenAI;
using DotNet.Practices.DynamicMapping.Dependencies;
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
    public async Task ResolveAsync_EveryPropertyGetsAValidConsistentValue(string fileName, string bagId)
    {
        var bag = PropertyBagFixtures.Read(fileName).Single(candidate => candidate.Id == bagId);
        var context = SampleContext.AdaLovelace;

        var result = await new LlmPropertyBagResolver(_chatClient!).ResolveAsync(bag, context);

        Log(bag, result);
        Assert.That(Violations(bag, context, result), Is.Empty);
    }

    // One assert, many rules: declared validators, lookups from the catalog, inputs copied literally,
    // plain copies honoured, and no properties the bag does not declare.
    private static IEnumerable<string> Violations(PropertyBag bag, MappingContext context, ResolvedBag result)
    {
        var declared = bag.Properties.Select(property => property.Id).ToHashSet(StringComparer.Ordinal);
        foreach (var extra in result.Values.Keys.Where(key => !declared.Contains(key)))
        {
            yield return $"{extra}: not declared in the bag";
        }

        foreach (var property in bag.Properties)
        {
            if (!result.Values.TryGetValue(property.Id, out var value))
            {
                yield return $"{property.Id}: missing";
                continue;
            }

            foreach (var error in PropertyValidator.Validate(property, value))
            {
                yield return $"{property.Id}: {error}";
            }

            if (property.Lookup is not null && value is not null && !LookupValues(context, property.Lookup).Contains(value))
            {
                yield return $"{property.Id}: '{value}' is not in {property.Lookup}";
            }

            if (IsInput(property) && context.Inputs.TryGetValue(property.Id, out var input) && value != input)
            {
                yield return $"{property.Id}: expected the requested input '{input}', got '{value}'";
            }

            if (IsInput(property) && !context.Inputs.ContainsKey(property.Id) && property.Type is PropertyType.Text or PropertyType.Date && value is not null)
            {
                yield return $"{property.Id}: invented '{value}' for an input nobody provided";
            }

            if (CopiedProperty(property) is { } source && value != result.Values.GetValueOrDefault(source))
            {
                yield return $"{property.Id}: should equal {source} ('{result.Values.GetValueOrDefault(source)}'), got '{value}'";
            }
        }
    }

    private static bool IsInput(PropertyDefinition property) =>
        property is { DefaultValue: null, ValueGenerator: null, IdentifyingGenerator: null, Lookup: null, Relation: null };

    private static string? CopiedProperty(PropertyDefinition property) =>
        property.ValueGenerator is { } generator && DependencyExtractor.Extract(generator) is { Count: 1 } single && generator.Trim() == $"$Context.{single.Single()}"
            ? single.Single()
            : null;

    private static IEnumerable<string> LookupValues(MappingContext context, string lookup) =>
        context.Options.TryGetValue(lookup, out var values) ? values.Split(',').Select(value => value.Trim()) : [];

    private static void Log(PropertyBag bag, ResolvedBag result)
    {
        TestContext.Out.WriteLine($"{bag.Id}  (tokens in/out: {result.Usage?.InputTokenCount}/{result.Usage?.OutputTokenCount})");
        foreach (var property in bag.Properties)
        {
            TestContext.Out.WriteLine($"  {property.Id,-30} = {result.Values.GetValueOrDefault(property.Id) ?? "(null)"}");
        }
    }
}
