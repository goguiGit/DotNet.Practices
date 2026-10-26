using DotNet.Practices.DynamicMapping.Models;
using DotNet.Practices.DynamicMapping.Tests.Fixtures;
using DotNet.Practices.DynamicMapping.Validators;

namespace DotNet.Practices.DynamicMapping.Tests.Schema;

public class SharedFolderFixtureTests
{
    private static readonly IReadOnlyList<PropertyBag> Bags = PropertyBagFixtures.Read("SharedFolder.propertybags.json");

    private static PropertyBag CustomInfo => Bags[0];

    private static PropertyBag ReadAccess => Bags[1];

    private static PropertyBag ReadWriteAccess => Bags[2];

    [Test]
    public void File_HasCustomInfoAndTwoAccessTypeBags()
    {
        Assert.That(Bags.Select(bag => bag.Id), Is.EqualTo(new[]
        {
            "SharedFolderCustomInfo", "SharedFolderReadAccessTypeProvisioning", "SharedFolderReadWriteAccessTypeProvisioning",
        }));
    }

    [Test]
    public void CustomInfo_DeclaresEightPropertiesInOrder()
    {
        Assert.That(CustomInfo.Properties.Select(property => property.Id), Is.EqualTo(new[]
        {
            "ShareRoot", "SharedFolderParent", "DisplayName", "Description", "Name", "Path", "Comments", "ResourceSequenceId",
        }));
    }

    [Test]
    public void SharedFolderParent_IsARelation()
    {
        Assert.That(CustomInfo.Property("SharedFolderParent"), Is.EqualTo(new PropertyDefinition("SharedFolderParent", PropertyType.Relation)
        {
            Relation = "SharedFolderParentChild",
        }));
    }

    [Test]
    public void DisplayName_IsReadOnlyAndBuiltFromParentOrRootPlusName()
    {
        Assert.That(CustomInfo.Property("DisplayName"), Is.EqualTo(new PropertyDefinition("DisplayName", PropertyType.Text)
        {
            ValueGenerator = "$(Context.SharedFolderParent == null ? Context.ShareRoot.Label : Context.SharedFolderParent.Label) + ' > ' + Context.Name",
            ReadOnly = true,
        }));
    }

    [Test]
    public void Path_IsReadOnlyAndBuiltFromParentPathOrRootPlusName()
    {
        Assert.That(CustomInfo.Property("Path"), Is.EqualTo(new PropertyDefinition("Path", PropertyType.Text)
        {
            ValueGenerator = @"$(Context.SharedFolderParent == null ? Context.ShareRoot.Key + '\\QAACC26\\' : Context.Relations?.SharedFolderParent?.Path) + '\\' + Context.Name",
            ReadOnly = true,
        }));
    }

    [Test]
    public void Name_IsRequiredWithPatternAndMaxLength128()
    {
        Assert.That(CustomInfo.Property("Name"), Is.EqualTo(new PropertyDefinition("Name", PropertyType.Text)
        {
            Validators = new PropertyValidators { Required = true, MaxLength = 128, Pattern = @"^[^\[\],:;\|=@\+\*\?<>/\\]+$" },
        }));
    }

    [Test]
    public void ResourceSequenceId_IsGeneratedUniqueAndReadOnly()
    {
        Assert.That(CustomInfo.Property("ResourceSequenceId"), Is.EqualTo(new PropertyDefinition("ResourceSequenceId", PropertyType.Text)
        {
            IdentifyingGenerator = "$Services.GetNextIdValue('{0:00000}', 1)",
            Unique = "SharedFolder.SequenceId",
            ReadOnly = true,
        }));
    }

    [Test]
    public void AccessTypeBags_ExtendAdObjectAccessTypePropertyBag()
    {
        Assert.That(Bags.Skip(1).Select(bag => bag.Extends.Single()), Is.All.EqualTo("AdObjectAccessTypePropertyBag"));
    }

    [Test]
    public void ReadAccess_OverridesGroupNameWithoutRedeclaringItsType()
    {
        Assert.That(ReadAccess.Property("GroupName"), Is.EqualTo(new PropertyDefinition("GroupName", Type: null)
        {
            DefaultValue = "$ $'GA_SF_QAACC26_{Context.ResourceSequenceId}_RO'",
        }));
    }

    [Test]
    public void ReadAccess_PinsAccessTypeId()
    {
        Assert.That(ReadAccess.Property("AccessTypeId").DefaultValue, Is.EqualTo("sf-read-access"));
    }

    [Test]
    public void ReadWriteAccess_PinsDomain()
    {
        Assert.That(ReadWriteAccess.Property("Domain").DefaultValue, Is.EqualTo("qaacc.int"));
    }

    [Test]
    public void ReadWriteAccess_SamAccountNameEqualsGroupName()
    {
        Assert.That(ReadWriteAccess.Property("SamAccountName").DefaultValue, Is.EqualTo(ReadWriteAccess.Property("GroupName").DefaultValue));
    }
}
