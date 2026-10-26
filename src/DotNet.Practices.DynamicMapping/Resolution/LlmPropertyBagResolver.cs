using System.Text;
using System.Text.Json;
using DotNet.Practices.DynamicMapping.Dependencies;
using DotNet.Practices.DynamicMapping.Models;
using DotNet.Practices.DynamicMapping.Resolution.Interfaces;
using Microsoft.Extensions.AI;

namespace DotNet.Practices.DynamicMapping.Resolution;

// Baseline resolver: the model evaluates every property in one call. Non-deterministic by nature;
// the compiled-plan path replaces it for anything that must be reproducible.
public sealed class LlmPropertyBagResolver(IChatClient chatClient) : IPropertyBagResolver
{
    private const string SystemInstruction =
        "You fill in the properties of a provisioning record from a declarative schema. " +
        "Evaluate properties in the given order, because later ones reference earlier ones through Context.<PropertyId>. " +
        "Expressions starting with $ are C#-like: Context.Owner.* is the external record, Context.Options.* and lookups are listed, " +
        "and Services.* functions are described in natural language. Properties without an expression are inputs: take them from the requested inputs and leave them null when there is none; never invent identifiers. Respect every validator. " +
        "Reply with a single JSON object mapping each property id to its value (string, or null when there is no sensible value). No prose, no markdown.";

    private const string ServicesSemantics =
        "Services.GetFiveTwoOneIdValue(lastname, firstname, addDigit, suffix): lowercase, first five letters of lastname, first two of firstname, then suffix. " +
        "Services.GetProvisioningContainer(mapping, key): the Options value ProvisioningMapping.<key>. " +
        "Services.GetIdToCheck(domainKey, sam): '<domainKey>\\<sam>'. " +
        "Services.GetNextIdValue(format, start): the next sequence number formatted with format, e.g. 00001. " +
        "Services.Combine(a, b): a and b joined with a single backslash. " +
        "A lookup value's .Key and .Label are the value itself.";

    private readonly IChatClient _chatClient = chatClient ?? throw new ArgumentNullException(nameof(chatClient));

    public async Task<ResolvedBag> ResolveAsync(PropertyBag bag, MappingContext context, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(bag);
        ArgumentNullException.ThrowIfNull(context);

        List<ChatMessage> messages =
        [
            new(ChatRole.System, SystemInstruction),
            new(ChatRole.User, BuildPrompt(bag, context)),
        ];

        var response = await _chatClient.GetResponseAsync(messages, cancellationToken: cancellationToken);
        return new ResolvedBag(bag.Id, ParseValues(response.Text), response.Text);
    }

    private static string BuildPrompt(PropertyBag bag, MappingContext context)
    {
        var graph = DependencyGraph.Build(bag);
        var prompt = new StringBuilder();

        prompt.AppendLine($"Record type: {bag.Id}");
        prompt.AppendLine().AppendLine("Owner (external record):");
        foreach (var (key, value) in context.Owner)
        {
            prompt.AppendLine($"  {key}: {value}");
        }

        prompt.AppendLine().AppendLine("Requested inputs (typed by the requester; use them for properties without an expression):");
        foreach (var (key, value) in context.Inputs)
        {
            prompt.AppendLine($"  {key}: {value}");
        }

        prompt.AppendLine().AppendLine("Options and lookups:");
        foreach (var (key, value) in context.Options)
        {
            prompt.AppendLine($"  {key}: {value}");
        }

        prompt.AppendLine().AppendLine("Services:").AppendLine($"  {ServicesSemantics}");
        prompt.AppendLine().AppendLine("Properties, in evaluation order:");
        foreach (var id in graph.EvaluationOrder())
        {
            prompt.AppendLine(Describe(bag.Properties.Single(property => property.Id == id), graph));
        }

        return prompt.ToString();
    }

    private static string Describe(PropertyDefinition property, DependencyGraph graph)
    {
        var parts = new List<string> { $"- {property.Id} ({property.Type?.ToString() ?? "inherited"})" };
        if (property.Lookup is not null)
        {
            parts.Add($"lookup: {property.Lookup}");
        }

        if (property.Relation is not null)
        {
            parts.Add($"relation: {property.Relation}");
        }

        if (property.ValueGenerator is not null)
        {
            parts.Add($"value: {property.ValueGenerator}");
        }
        else if (property.IdentifyingGenerator is not null)
        {
            parts.Add($"value: {property.IdentifyingGenerator}");
        }
        else if (property.DefaultValue is not null)
        {
            parts.Add($"default: {property.DefaultValue}");
        }

        var validators = property.Validators;
        var rules = new List<string>();
        if (validators.Required)
        {
            rules.Add("required");
        }

        if (validators.Pattern is not null)
        {
            rules.Add($"pattern: {validators.Pattern}");
        }

        if (validators.MaxLength is { } maxLength)
        {
            rules.Add($"maxlength: {maxLength}");
        }

        if (validators.FutureDate)
        {
            rules.Add("must be a future date");
        }

        if (rules.Count > 0)
        {
            parts.Add($"validators: {string.Join(", ", rules)}");
        }

        var dependencies = graph.DependenciesOf(property.Id);
        if (dependencies.Count > 0)
        {
            parts.Add($"depends on: {string.Join(", ", dependencies)}");
        }

        if (property.ReadOnly)
        {
            parts.Add("readOnly");
        }

        return string.Join(" | ", parts);
    }

    private static IReadOnlyDictionary<string, string?> ParseValues(string response)
    {
        var json = response.Trim();
        if (json.StartsWith("```", StringComparison.Ordinal))
        {
            json = json[json.IndexOf('\n')..].TrimEnd().TrimEnd('`').Trim();
        }

        try
        {
            using var document = JsonDocument.Parse(json);
            if (document.RootElement.ValueKind != JsonValueKind.Object)
            {
                throw new FormatException("The model did not return a JSON object.");
            }

            return document.RootElement.EnumerateObject().ToDictionary(
                property => property.Name,
                property => property.Value.ValueKind switch
                {
                    JsonValueKind.Null => null,
                    JsonValueKind.String => property.Value.GetString(),
                    _ => property.Value.GetRawText(),
                },
                StringComparer.Ordinal);
        }
        catch (JsonException exception)
        {
            throw new FormatException("The model did not return valid JSON.", exception);
        }
    }
}
