using DotNet.Practices.DynamicMapping.Models;
using DotNet.Practices.DynamicMapping.Tests.Fixtures;
using DotNet.Practices.DynamicMapping.Validators;

namespace DotNet.Practices.DynamicMapping.Tests.Schema;

public class DistributionGroupFixtureTests
{
    private static readonly IReadOnlyList<PropertyBag> Bags = PropertyBagFixtures.Read("DistributionGroup.propertybags.json");

    private static PropertyBag CustomInfo => Bags[0];

    [Test]
    public void File_HasCustomInfoAndThreeAccessTypeBags()
    {
        Assert.That(Bags.Select(bag => bag.Id), Is.EqualTo(new[]
        {
            "DistributionGroupCustomInfo",
            "DistributionGroupAllowedToSendAccessTypeProvisioning",
            "DistributionGroupMemberAccessTypeProvisioning",
            "DistributionGroupSendAsAccessTypeProvisioning",
        }));
    }

    [Test]
    public void CustomInfo_DeclaresFourteenProperties()
    {
        Assert.That(CustomInfo.Properties, Has.Count.EqualTo(14));
    }

    [TestCase("HiddenFromAddressBook", "false")]
    [TestCase("RequiresSenderAuthentication", "true")]
    [TestCase("SecurityEnabled", "false")]
    public void BooleanFlag_HasLiteralDefault(string id, string defaultValue)
    {
        Assert.That(CustomInfo.Property(id), Is.EqualTo(new PropertyDefinition(id, PropertyType.Boolean) { DefaultValue = defaultValue }));
    }

    [Test]
    public void PrimarySmtpAddress_IsUniqueReadOnlyAndComposedFromLocalAndDomainParts()
    {
        Assert.That(CustomInfo.Property("PrimarySmtpAddress"), Is.EqualTo(new PropertyDefinition("PrimarySmtpAddress", PropertyType.Text)
        {
            Unique = "SmtpAddress",
            ValueGenerator = "$'DD_QAACC26_' + Context.PrimarySmtpAddressLocalPart + '@' + Context.PrimarySmtpAddressDomainPart?.Key",
            ReadOnly = true,
        }));
    }

    [Test]
    public void SecondarySmtpAddresses_ShareTheUniquenessScopeWithPrimary()
    {
        Assert.That(CustomInfo.Property("SecondarySmtpAddresses").Unique, Is.EqualTo("SmtpAddress"));
    }

    [TestCase("Description", 255)]
    [TestCase("MailTip", 175)]
    public void OptionalText_IsCapped(string id, int maxLength)
    {
        Assert.That(CustomInfo.Property(id), Is.EqualTo(new PropertyDefinition(id, PropertyType.Text)
        {
            Validators = new PropertyValidators { MaxLength = maxLength },
        }));
    }

    [Test]
    public void Name_IsRequiredAndCappedAt64()
    {
        Assert.That(CustomInfo.Property("Name"), Is.EqualTo(new PropertyDefinition("Name", PropertyType.Text)
        {
            Validators = new PropertyValidators { Required = true, MaxLength = 64 },
        }));
    }

    [Test]
    public void ResourceSequenceId_IsGeneratedUniqueAndReadOnly()
    {
        Assert.That(CustomInfo.Property("ResourceSequenceId"), Is.EqualTo(new PropertyDefinition("ResourceSequenceId", PropertyType.Text)
        {
            IdentifyingGenerator = "$Services.GetNextIdValue('{0:00000}', 1)",
            Unique = "DistributionGroup.SequenceId",
            ReadOnly = true,
        }));
    }

    [Test]
    public void AccessTypeBags_AllExtendAdObjectAccessTypePropertyBag()
    {
        Assert.That(Bags.Skip(1).Select(bag => bag.Extends.Single()), Is.All.EqualTo("AdObjectAccessTypePropertyBag"));
    }

    [TestCase(1, "dg-allowedtosend-access")]
    [TestCase(2, "dg-member-access")]
    [TestCase(3, "dg-sendas-access")]
    public void AccessTypeBag_PinsAccessTypeId(int index, string accessTypeId)
    {
        Assert.That(Bags[index].Property("AccessTypeId"), Is.EqualTo(new PropertyDefinition("AccessTypeId", Type: null) { DefaultValue = accessTypeId }));
    }

    [TestCase(1, "$ $'GA_DG_QAACC26_{Context.ResourceSequenceId}_Send'")]
    [TestCase(2, "$$'DD_QAACC26_{Context.Name}'")]
    [TestCase(3, "$ $'GA_DG_QAACC26_{Context.ResourceSequenceId}_SendAs'")]
    public void AccessTypeBag_OverridesGroupNameWithoutRedeclaringItsType(int index, string groupName)
    {
        Assert.That(Bags[index].Property("GroupName"), Is.EqualTo(new PropertyDefinition("GroupName", Type: null) { DefaultValue = groupName }));
    }

    [Test]
    public void MemberAccess_IsTheOnlyOneWithoutDistinguishedNameOverride()
    {
        Assert.That(Bags.Skip(1).Select(bag => bag.Properties.Any(property => property.Id == "DistinguishedName")), Is.EqualTo(new[] { true, false, true }));
    }
}
