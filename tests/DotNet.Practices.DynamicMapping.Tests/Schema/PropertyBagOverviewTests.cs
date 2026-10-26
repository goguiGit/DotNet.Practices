using DotNet.Practices.DynamicMapping.Dependencies;
using DotNet.Practices.DynamicMapping.Models;
using DotNet.Practices.DynamicMapping.Tests.Fixtures;

namespace DotNet.Practices.DynamicMapping.Tests.Schema;

public class PropertyBagOverviewTests
{
    public static IEnumerable<TestCaseData> AllBags =>
        PropertyBagFixtures.AllFileNames.SelectMany(fileName => PropertyBagFixtures.Read(fileName)
            .Select(bag => new TestCaseData(fileName, bag.Id).SetName($"{fileName}/{bag.Id}")));

    [TestCaseSource(nameof(AllBags))]
    public void Overview_EveryPropertyIsEvaluable(string fileName, string bagId)
    {
        var bag = PropertyBagFixtures.Read(fileName).Single(candidate => candidate.Id == bagId);
        var graph = DependencyGraph.Build(bag);
        var order = graph.EvaluationOrder();

        Log(bag, graph, order);

        Assert.That(order, Is.EquivalentTo(bag.Properties.Select(property => property.Id)));
    }

    private static void Log(PropertyBag bag, DependencyGraph graph, IReadOnlyList<string> order)
    {
        var log = TestContext.Out;
        log.WriteLine($"{bag.Id}{(bag.Extends.Count > 0 ? $" extends {string.Join(", ", bag.Extends)}" : "")}");
        log.WriteLine($"  {"property",-30} {"type",-16} {"source",-20} {"depends on",-32} external");
        foreach (var id in order)
        {
            var property = bag.Properties.Single(candidate => candidate.Id == id);
            log.WriteLine($"  {id,-30} {property.Type?.ToString() ?? "(base)",-16} {Source(property),-20} {Join(graph.DependenciesOf(id)),-32} {Join(graph.ExternalReferencesOf(id))}");
        }

        log.WriteLine();
    }

    private static string Source(PropertyDefinition property) => property switch
    {
        { ValueGenerator: not null } => "valueGenerator",
        { IdentifyingGenerator: not null } => "identifyingGenerator",
        { DefaultValue: { } value } when value.StartsWith('$') => "defaultValue ($)",
        { DefaultValue: not null } => "defaultValue",
        { Lookup: not null } => "lookup",
        { Relation: not null } => "relation",
        _ => "input",
    };

    private static string Join(IEnumerable<string> ids) => string.Join(", ", ids.Order(StringComparer.Ordinal));
}
