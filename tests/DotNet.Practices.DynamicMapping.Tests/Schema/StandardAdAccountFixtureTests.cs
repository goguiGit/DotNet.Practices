using DotNet.Practices.DynamicMapping.Models;
using DotNet.Practices.DynamicMapping.Tests.Fixtures;
using DotNet.Practices.DynamicMapping.Validators;

namespace DotNet.Practices.DynamicMapping.Tests.Schema;

public class StandardAdAccountFixtureTests
{
    private static readonly PropertyBag Bag = PropertyBagFixtures.Read("StandardAdAccount.propertybags.json").Single();

    [Test]
    public void Bag_IsStandardAdAccountDataWithThirteenProperties()
    {
        Assert.Multiple(() =>
        {
            Assert.That(Bag.Id, Is.EqualTo("StandardAdAccountData"));
            Assert.That(Bag.Properties, Has.Count.EqualTo(13));
            Assert.That(Bag.Extends, Is.Empty);
        });
    }

    [Test]
    public void SamAccountName_UsesStandardSuffixInsteadOfAdminOne()
    {
        var samAccountName = Bag.Property("SamAccountName");

        Assert.Multiple(() =>
        {
            Assert.That(samAccountName.DefaultValue, Is.EqualTo("$Services.GetFiveTwoOneIdValue(Context.Owner.Lastname, Context.Owner.Firstname, false, '26')"));
            Assert.That(samAccountName.Unique, Is.EqualTo("ADAccountData"));
            Assert.That(samAccountName.Validators.MaxLength, Is.EqualTo(20));
        });
    }

    [Test]
    public void DistinguishedName_UsesStandardContainer()
    {
        Assert.That(Bag.Property("DistinguishedName").ValueGenerator, Does.Contain("'StandardAdAccountContainer'"));
    }

    [Test]
    public void HomeDirectory_IsRequiredLookup()
    {
        var homeDirectory = Bag.Property("HomeDirectory");

        Assert.Multiple(() =>
        {
            Assert.That(homeDirectory.Type, Is.EqualTo(PropertyType.Lookup));
            Assert.That(homeDirectory.Lookup, Is.EqualTo("Lookup.HomeDirectory"));
            Assert.That(homeDirectory.Validators.Required, Is.True);
        });
    }

    [Test]
    public void HomeDirectoryInAd_DependsOnHomeDirectoryAndSamAccountName()
    {
        var homeDirectoryInAd = Bag.Property("HomeDirectoryInAd");

        Assert.Multiple(() =>
        {
            Assert.That(homeDirectoryInAd.Type, Is.EqualTo(PropertyType.Text));
            Assert.That(homeDirectoryInAd.DefaultValue, Is.EqualTo("$Services.Combine(Context.HomeDirectory.Key, Context.SamAccountName)"));
            Assert.That(homeDirectoryInAd.ReadOnly, Is.False);
        });
    }

    [Test]
    public void UPNOverride_HasOnlyUnsupportedValidators_SoItReadsAsNone()
    {
        // The file declares "emailDomain": true, which the reader does not model yet.
        Assert.That(Bag.Property("UPNOverride").Validators, Is.EqualTo(PropertyValidators.None));
    }

    [Test]
    public void SharesTheCoreAccountPropertiesWithAdminAdAccount()
    {
        var admin = PropertyBagFixtures.Read("AdminAdAccount.propertybags.json").Single();

        var shared = Bag.Properties.Select(property => property.Id).Intersect(admin.Properties.Select(property => property.Id));

        Assert.That(shared, Is.EquivalentTo(new[]
        {
            "Domain", "SamAccountName", "DisplayName", "AdGuid", "DistinguishedName", "IdForTargetSystem",
            "UPNOverride", "StartDate", "EndDate", "Comments", "Description",
        }));
    }
}
