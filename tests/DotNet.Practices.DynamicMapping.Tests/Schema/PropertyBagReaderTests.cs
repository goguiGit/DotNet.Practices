using DotNet.Practices.DynamicMapping.Models;
using DotNet.Practices.DynamicMapping.Schema;
using DotNet.Practices.DynamicMapping.Tests.Fixtures;

namespace DotNet.Practices.DynamicMapping.Tests.Schema;

public class PropertyBagReaderTests
{
    [TestCaseSource(typeof(PropertyBagFixtures), nameof(PropertyBagFixtures.AllFileNames))]
    public void ReadFile_RealFixture_EveryBagHasAnIdAndProperties(string fileName)
    {
        var bags = PropertyBagFixtures.Read(fileName);

        Assert.That(bags.Select(bag => (bag.Id.Length > 0, bag.Properties.Count > 0)), Is.All.EqualTo((true, true)));
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

        Assert.That(PropertyBagReader.Read(json)[0].Property("A"), Is.EqualTo(new PropertyDefinition("A", PropertyType.Text)));
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

    [TestCase("true", "true")]
    [TestCase("3", "3")]
    [TestCase("\"text\"", "text")]
    public void Read_DefaultValue_IsKeptAsText(string jsonValue, string expected)
    {
        var json = $$"""[{ "id": "Bag", "properties": [{ "id": "A", "type": "text", "defaultValue": {{jsonValue}} }] }]""";

        Assert.That(PropertyBagReader.Read(json)[0].Property("A").DefaultValue, Is.EqualTo(expected));
    }

    [TestCase("20")]
    [TestCase("\"20\"")]
    public void Read_MaxLengthAsNumberOrNumericString_IsRead(string jsonValue)
    {
        var json = $$"""[{ "id": "Bag", "properties": [{ "id": "A", "type": "text", "validators": { "maxlength": {{jsonValue}} } }] }]""";

        Assert.That(PropertyBagReader.Read(json)[0].Property("A").Validators.MaxLength, Is.EqualTo(20));
    }

    [Test]
    public void Read_MaxLengthThatIsNotANumber_ThrowsFormatException()
    {
        const string json = """[{ "id": "Bag", "properties": [{ "id": "A", "type": "text", "validators": { "maxlength": "many" } }] }]""";

        Assert.That(() => PropertyBagReader.Read(json), Throws.TypeOf<FormatException>().With.Message.Contains("A"));
    }

    [TestCase("\"extends\": \"Base\"")]
    [TestCase("\"properties\": {}")]
    public void Read_ExtendsOrPropertiesNotAnArray_ThrowsFormatException(string fragment)
    {
        var json = $$"""[{ "id": "Bag", {{fragment}} }]""";

        Assert.That(() => PropertyBagReader.Read(json), Throws.TypeOf<FormatException>());
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
