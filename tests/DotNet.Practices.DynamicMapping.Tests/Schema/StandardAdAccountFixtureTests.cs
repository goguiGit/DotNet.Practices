using DotNet.Practices.DynamicMapping.Models;
using DotNet.Practices.DynamicMapping.Tests.Fixtures;
using DotNet.Practices.DynamicMapping.Validators;

namespace DotNet.Practices.DynamicMapping.Tests.Schema;

public class StandardAdAccountFixtureTests
{
    private static readonly PropertyBag Bag = PropertyBagFixtures.Read("StandardAdAccount.propertybags.json").Single();

    [Test]
    public void Bag_IsStandardAdAccountData()
    {
        Assert.That(Bag.Id, Is.EqualTo("StandardAdAccountData"));
    }

    [Test]
    public void Bag_DeclaresThirteenPropertiesInOrder()
    {
        Assert.That(Bag.Properties.Select(property => property.Id), Is.EqualTo(new[]
        {
            "Domain", "SamAccountName", "DisplayName", "AdGuid", "DistinguishedName", "IdForTargetSystem",
            "UPNOverride", "StartDate", "EndDate", "Comments", "Description", "HomeDirectory", "HomeDirectoryInAd",
        }));
    }

    [Test]
    public void SamAccountName_UsesStandardSuffixInsteadOfAdminOne()
    {
        Assert.That(Bag.Property("SamAccountName"), Is.EqualTo(new PropertyDefinition("SamAccountName", PropertyType.Text)
        {
            Unique = "ADAccountData",
            DefaultValue = "$Services.GetFiveTwoOneIdValue(Context.Owner.Lastname, Context.Owner.Firstname, false, '26')",
            Validators = new PropertyValidators { Required = true, MaxLength = 20, Pattern = @"^[^\[\],:;\|=@\+\*\?<>/\\]+$" },
        }));
    }

    [Test]
    public void DistinguishedName_UsesStandardContainer()
    {
        Assert.That(Bag.Property("DistinguishedName"), Is.EqualTo(new PropertyDefinition("DistinguishedName", PropertyType.Text)
        {
            ValueGenerator = "$ $'CN={Context.SamAccountName},{Services.GetProvisioningContainer(Context.Options.ProvisioningMapping.Values, 'StandardAdAccountContainer')}'",
            ReadOnly = true,
        }));
    }

    [Test]
    public void HomeDirectory_IsRequiredLookup()
    {
        Assert.That(Bag.Property("HomeDirectory"), Is.EqualTo(new PropertyDefinition("HomeDirectory", PropertyType.Lookup)
        {
            Lookup = "Lookup.HomeDirectory",
            Validators = new PropertyValidators { Required = true },
        }));
    }

    [Test]
    public void HomeDirectoryInAd_CombinesHomeDirectoryAndSamAccountName()
    {
        Assert.That(Bag.Property("HomeDirectoryInAd"), Is.EqualTo(new PropertyDefinition("HomeDirectoryInAd", PropertyType.Text)
        {
            DefaultValue = "$Services.Combine(Context.HomeDirectory.Key, Context.SamAccountName)",
        }));
    }

    [Test]
    public void UPNOverride_OnlyHasValidatorsTheReaderDoesNotModelYet()
    {
        Assert.That(Bag.Property("UPNOverride"), Is.EqualTo(new PropertyDefinition("UPNOverride", PropertyType.Text)));
    }
}
