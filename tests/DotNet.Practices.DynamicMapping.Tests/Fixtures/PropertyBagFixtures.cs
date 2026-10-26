using DotNet.Practices.DynamicMapping.Models;
using DotNet.Practices.DynamicMapping.Schema;

namespace DotNet.Practices.DynamicMapping.Tests.Fixtures;

// Real propertybags.json files copied from a Rabbit environment; one test class per file asserts its shape.
public static class PropertyBagFixtures
{
    public static readonly string Directory = Path.Combine(TestContext.CurrentContext.TestDirectory, "Fixtures", "PropertyBags");

    public static IEnumerable<string> AllFileNames => System.IO.Directory.EnumerateFiles(Directory, "*.propertybags.json").Select(Path.GetFileName)!;

    public static IReadOnlyList<PropertyBag> Read(string fileName) => PropertyBagReader.ReadFile(Path.Combine(Directory, fileName));

    public static PropertyDefinition Property(this PropertyBag bag, string id) => bag.Properties.Single(property => property.Id == id);
}
