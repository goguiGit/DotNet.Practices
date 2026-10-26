using DotNet.Practices.DynamicMapping.Models;
using DotNet.Practices.DynamicMapping.Tests.Fixtures;

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
    public void CustomInfo_HasFourteenTypedProperties()
    {
        Assert.Multiple(() =>
        {
            Assert.That(CustomInfo.Properties, Has.Count.EqualTo(14));
            Assert.That(CustomInfo.Properties.Select(property => property.Type), Is.All.Not.Null);
        });
    }

    [Test]
    public void BooleanFlags_HaveLiteralDefaults()
    {
        Assert.Multiple(() =>
        {
            Assert.That(CustomInfo.Property("HiddenFromAddressBook").Type, Is.EqualTo(PropertyType.Boolean));
            Assert.That(CustomInfo.Property("HiddenFromAddressBook").DefaultValue, Is.EqualTo("false"));
            Assert.That(CustomInfo.Property("RequiresSenderAuthentication").DefaultValue, Is.EqualTo("true"));
            Assert.That(CustomInfo.Property("SecurityEnabled").DefaultValue, Is.EqualTo("false"));
        });
    }

    [Test]
    public void PrimarySmtpAddress_IsUniqueReadOnlyAndComposedFromLocalAndDomainParts()
    {
        var primary = CustomInfo.Property("PrimarySmtpAddress");

        Assert.Multiple(() =>
        {
            Assert.That(primary.Unique, Is.EqualTo("SmtpAddress"));
            Assert.That(primary.ReadOnly, Is.True);
            Assert.That(primary.ValueGenerator, Does.Contain("Context.PrimarySmtpAddressLocalPart"));
            Assert.That(primary.ValueGenerator, Does.Contain("Context.PrimarySmtpAddressDomainPart?.Key"));
        });
    }

    [Test]
    public void SecondarySmtpAddresses_ShareTheUniquenessScopeWithPrimary()
    {
        Assert.That(CustomInfo.Property("SecondarySmtpAddresses").Unique, Is.EqualTo(CustomInfo.Property("PrimarySmtpAddress").Unique));
    }

    [Test]
    public void TextLengths_AreCapped()
    {
        Assert.Multiple(() =>
        {
            Assert.That(CustomInfo.Property("Description").Validators.MaxLength, Is.EqualTo(255));
            Assert.That(CustomInfo.Property("Name").Validators.MaxLength, Is.EqualTo(64));
            Assert.That(CustomInfo.Property("MailTip").Validators.MaxLength, Is.EqualTo(175));
        });
    }

    [Test]
    public void AccessTypeBags_AllExtendAdObjectAccessTypePropertyBag()
    {
        Assert.That(Bags.Skip(1).Select(bag => bag.Extends.Single()), Is.All.EqualTo("AdObjectAccessTypePropertyBag"));
    }

    [TestCase(1, "dg-allowedtosend-access", "$ $'GA_DG_QAACC26_{Context.ResourceSequenceId}_Send'")]
    [TestCase(2, "dg-member-access", "$$'DD_QAACC26_{Context.Name}'")]
    [TestCase(3, "dg-sendas-access", "$ $'GA_DG_QAACC26_{Context.ResourceSequenceId}_SendAs'")]
    public void AccessTypeBag_OverridesAccessTypeIdAndGroupName(int index, string accessTypeId, string groupName)
    {
        var bag = Bags[index];

        Assert.Multiple(() =>
        {
            Assert.That(bag.Property("AccessTypeId").DefaultValue, Is.EqualTo(accessTypeId));
            Assert.That(bag.Property("GroupName").DefaultValue, Is.EqualTo(groupName));
            Assert.That(bag.Property("GroupName").Type, Is.Null, "type comes from the base bag");
        });
    }

    [Test]
    public void MemberAccess_IsTheOnlyOneWithoutDistinguishedNameOverride()
    {
        Assert.Multiple(() =>
        {
            Assert.That(Bags[2].Properties.Select(property => property.Id), Does.Not.Contain("DistinguishedName"));
            Assert.That(Bags[1].Properties.Select(property => property.Id), Does.Contain("DistinguishedName"));
            Assert.That(Bags[3].Properties.Select(property => property.Id), Does.Contain("DistinguishedName"));
        });
    }
}
