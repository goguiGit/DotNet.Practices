using DotNet.Practices.DynamicMapping.Models;
using DotNet.Practices.DynamicMapping.Schema;
using DotNet.Practices.DynamicMapping.Tests.Fixtures;

namespace DotNet.Practices.DynamicMapping.Tests.Schema;

public class PropertyBagReaderTests
{
    [TestCaseSource(typeof(PropertyBagFixtures), nameof(PropertyBagFixtures.AllFileNames))]
    public void ReadFile_RealFixture_ParsesEveryBagWithIdAndProperties(string fileName)
    {
        var bags = PropertyBagFixtures.Read(fileName);

        Assert.Multiple(() =>
        {
            Assert.That(bags, Is.Not.Empty);
            Assert.That(bags.Select(bag => bag.Id), Is.All.Not.Empty);
            Assert.That(bags.Select(bag => bag.Properties.Count), Is.All.GreaterThan(0));
        });
    }

    [Test]
    public void Read_EmptyArray_ReturnsNoBags()
    {
        Assert.That(PropertyBagReader.Read("[]"), Is.Empty);
    }

    [Test]
    public void Read_RootIsNotAnArray_ThrowsFormatException()
    {
        Assert.That(() => PropertyBagReader.Read("""{ "id": "Bag" }"""), Throws.TypeOf<FormatException>());
    }

    [Test]
    public void Read_ToleratesTrailingComma()
    {
        const string json = """[{ "id": "Bag", "properties": [{ "id": "A", "type": "text", }], }]""";

        Assert.That(PropertyBagReader.Read(json)[0].Property("A").Type, Is.EqualTo(PropertyType.Text));
    }

    [TestCase("readonly")]
    [TestCase("readOnly")]
    [TestCase("ReadOnly")]
    public void Read_ReadOnlyKeyIsCaseInsensitive(string key)
    {
        var json = $$"""[{ "id": "Bag", "properties": [{ "id": "A", "type": "text", "{{key}}": true }] }]""";

        Assert.That(PropertyBagReader.Read(json)[0].Property("A").ReadOnly, Is.True);
    }

    [Test]
    public void Read_NameAsPlainStringOrObject_IsIgnored()
    {
        const string json = """[{ "id": "Bag", "properties": [{ "id": "A", "name": "Plain", "type": "text" }, { "id": "B", "name": { "": "Obj", "de": "Objekt" }, "type": "text" }] }]""";

        Assert.That(PropertyBagReader.Read(json)[0].Properties.Select(property => property.Id), Is.EqualTo(new[] { "A", "B" }));
    }

    [Test]
    public void Read_NonStringDefaultValue_IsKeptAsJsonText()
    {
        const string json = """[{ "id": "Bag", "properties": [{ "id": "Flag", "type": "boolean", "defaultValue": true }, { "id": "Count", "type": "integer", "defaultValue": 3 }] }]""";

        var bag = PropertyBagReader.Read(json)[0];

        Assert.Multiple(() =>
        {
            Assert.That(bag.Property("Flag").DefaultValue, Is.EqualTo("true"));
            Assert.That(bag.Property("Count").DefaultValue, Is.EqualTo("3"));
        });
    }

    [Test]
    public void Read_UnknownPropertyType_ThrowsFormatExceptionNamingTheProperty()
    {
        const string json = """[{ "id": "Bag", "properties": [{ "id": "Weird", "type": "hologram" }] }]""";

        Assert.That(() => PropertyBagReader.Read(json), Throws.TypeOf<FormatException>().With.Message.Contains("Weird"));
    }

    [Test]
    public void Read_PropertyWithoutId_ThrowsFormatException()
    {
        const string json = """[{ "id": "Bag", "properties": [{ "type": "text" }] }]""";

        Assert.That(() => PropertyBagReader.Read(json), Throws.TypeOf<FormatException>());
    }

    [Test]
    public void Read_BagWithoutId_ThrowsFormatException()
    {
        Assert.That(() => PropertyBagReader.Read("""[{ "properties": [] }]"""), Throws.TypeOf<FormatException>());
    }
}
