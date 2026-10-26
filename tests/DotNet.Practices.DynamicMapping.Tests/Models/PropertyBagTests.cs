using DotNet.Practices.DynamicMapping.Models;

namespace DotNet.Practices.DynamicMapping.Tests.Models;

public class PropertyBagTests
{
    [Test]
    public void Constructor_WithDuplicatePropertyIds_ThrowsArgumentExceptionNamingTheId()
    {
        var duplicate = new PropertyDefinition("Name", PropertyType.Text);

        Assert.That(() => new PropertyBag("Bag", [duplicate, duplicate]), Throws.ArgumentException.With.Message.Contains("Name"));
    }

    [Test]
    public void Constructor_WithBlankId_ThrowsArgumentException()
    {
        Assert.That(() => new PropertyBag(" ", []), Throws.ArgumentException);
    }
}
