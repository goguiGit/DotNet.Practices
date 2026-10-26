using DotNet.Practices.DynamicMapping.Models;
using DotNet.Practices.DynamicMapping.Tests.Fixtures;

namespace DotNet.Practices.DynamicMapping.Tests.Schema;

public class SharedFolderFixtureTests
{
    private static readonly IReadOnlyList<PropertyBag> Bags = PropertyBagFixtures.Read("SharedFolder.propertybags.json");

    private static PropertyBag CustomInfo => Bags[0];

    [Test]
    public void File_HasCustomInfoAndTwoAccessTypeBags()
    {
        Assert.That(Bags.Select(bag => bag.Id), Is.EqualTo(new[]
        {
            "SharedFolderCustomInfo",
            "SharedFolderReadAccessTypeProvisioning",
            "SharedFolderReadWriteAccessTypeProvisioning",
        }));
    }

    [Test]
    public void CustomInfo_HasEightTypedPropertiesAndNoBase()
    {
        Assert.Multiple(() =>
        {
            Assert.That(CustomInfo.Properties, Has.Count.EqualTo(8));
            Assert.That(CustomInfo.Extends, Is.Empty);
            Assert.That(CustomInfo.Properties.Select(property => property.Type), Is.All.Not.Null);
        });
    }

    [Test]
    public void SharedFolderParent_IsARelation()
    {
        var parent = CustomInfo.Property("SharedFolderParent");

        Assert.Multiple(() =>
        {
            Assert.That(parent.Type, Is.EqualTo(PropertyType.Relation));
            Assert.That(parent.Relation, Is.EqualTo("SharedFolderParentChild"));
        });
    }

    [Test]
    public void DisplayNameAndPath_AreReadOnlyAndDependOnParentRootAndName()
    {
        foreach (var id in new[] { "DisplayName", "Path" })
        {
            var property = CustomInfo.Property(id);
            Assert.Multiple(() =>
            {
                Assert.That(property.ReadOnly, Is.True, id);
                Assert.That(property.ValueGenerator, Does.Contain("Context.SharedFolderParent"), id);
                Assert.That(property.ValueGenerator, Does.Contain("Context.ShareRoot"), id);
                Assert.That(property.ValueGenerator, Does.Contain("Context.Name"), id);
            });
        }
    }

    [Test]
    public void Name_IsRequiredWithPatternAndMaxLength128()
    {
        var name = CustomInfo.Property("Name");

        Assert.Multiple(() =>
        {
            Assert.That(name.Validators.Required, Is.True);
            Assert.That(name.Validators.MaxLength, Is.EqualTo(128));
            Assert.That(name.Validators.Pattern, Is.Not.Null);
        });
    }

    [Test]
    public void ResourceSequenceId_IsGeneratedUniqueAndReadOnly()
    {
        var sequenceId = CustomInfo.Property("ResourceSequenceId");

        Assert.Multiple(() =>
        {
            Assert.That(sequenceId.IdentifyingGenerator, Is.EqualTo("$Services.GetNextIdValue('{0:00000}', 1)"));
            Assert.That(sequenceId.Unique, Is.EqualTo("SharedFolder.SequenceId"));
            Assert.That(sequenceId.ReadOnly, Is.True);
            Assert.That(sequenceId.DefaultValue, Is.Null);
        });
    }

    [Test]
    public void AccessTypeBags_ExtendAdObjectAccessTypePropertyBagWithUntypedOverrides()
    {
        foreach (var bag in Bags.Skip(1))
        {
            Assert.Multiple(() =>
            {
                Assert.That(bag.Extends, Is.EqualTo(new[] { "AdObjectAccessTypePropertyBag" }), bag.Id);
                Assert.That(bag.Properties.Select(property => property.Type), Is.All.Null, bag.Id);
                Assert.That(bag.Properties.Select(property => property.DefaultValue), Is.All.Not.Null, bag.Id);
            });
        }
    }

    [Test]
    public void ReadAccess_OverridesGroupNameDistinguishedNameAndAccessTypeId()
    {
        var readAccess = Bags[1];

        Assert.Multiple(() =>
        {
            Assert.That(readAccess.Properties.Select(property => property.Id), Is.EqualTo(new[] { "GroupName", "DistinguishedName", "AccessTypeId" }));
            Assert.That(readAccess.Property("GroupName").DefaultValue, Is.EqualTo("$ $'GA_SF_QAACC26_{Context.ResourceSequenceId}_RO'"));
            Assert.That(readAccess.Property("AccessTypeId").DefaultValue, Is.EqualTo("sf-read-access"));
        });
    }

    [Test]
    public void ReadWriteAccess_AlsoPinsDomainAndSamAccountName()
    {
        var readWrite = Bags[2];

        Assert.Multiple(() =>
        {
            Assert.That(readWrite.Property("Domain").DefaultValue, Is.EqualTo("qaacc.int"));
            Assert.That(readWrite.Property("SamAccountName").DefaultValue, Is.EqualTo(readWrite.Property("GroupName").DefaultValue));
            Assert.That(readWrite.Property("AccessTypeId").DefaultValue, Is.EqualTo("sf-read-write-access"));
        });
    }
}
