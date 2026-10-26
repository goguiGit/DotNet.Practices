using DotNet.Practices.DynamicMapping.Models;
using DotNet.Practices.DynamicMapping.Tests.Fixtures;
using DotNet.Practices.DynamicMapping.Validators;

namespace DotNet.Practices.DynamicMapping.Tests.Schema;

public class AdminAdAccountFixtureTests
{
    private static readonly PropertyBag Bag = PropertyBagFixtures.Read("AdminAdAccount.propertybags.json").Single();

    [Test]
    public void Bag_IsAdminAdAccountData()
    {
        Assert.That(Bag.Id, Is.EqualTo("AdminAdAccountData"));
    }

    [Test]
    public void Bag_HasNoBase()
    {
        Assert.That(Bag.Extends, Is.Empty);
    }

    [Test]
    public void Bag_DeclaresElevenPropertiesInOrder()
    {
        Assert.That(Bag.Properties.Select(property => property.Id), Is.EqualTo(new[]
        {
            "Domain", "SamAccountName", "DisplayName", "AdGuid", "DistinguishedName", "IdForTargetSystem",
            "UPNOverride", "StartDate", "EndDate", "Comments", "Description",
        }));
    }

    [Test]
    public void Domain_IsRequiredLookup()
    {
        Assert.That(Bag.Property("Domain"), Is.EqualTo(new PropertyDefinition("Domain", PropertyType.Lookup)
        {
            Lookup = "Lookup.AdAccountDomain",
            Validators = new PropertyValidators { Required = true },
        }));
    }

    [Test]
    public void SamAccountName_IsUniqueTextDerivedFromOwnerWithStrictValidators()
    {
        Assert.That(Bag.Property("SamAccountName"), Is.EqualTo(new PropertyDefinition("SamAccountName", PropertyType.Text)
        {
            Unique = "ADAccountData",
            DefaultValue = "$Services.GetFiveTwoOneIdValue(Context.Owner.Lastname, Context.Owner.Firstname, false, '-adm-26')",
            Validators = new PropertyValidators { Required = true, MaxLength = 20, Pattern = @"^[^\[\],:;\|=@\+\*\?<>/\\]+$" },
        }));
    }

    [Test]
    public void DisplayName_IsReadOnlyCopyOfSamAccountName()
    {
        Assert.That(Bag.Property("DisplayName"), Is.EqualTo(new PropertyDefinition("DisplayName", PropertyType.Text)
        {
            ValueGenerator = "$Context.SamAccountName",
            ReadOnly = true,
        }));
    }

    [Test]
    public void AdGuid_IsUniqueIdentifier()
    {
        Assert.That(Bag.Property("AdGuid"), Is.EqualTo(new PropertyDefinition("AdGuid", PropertyType.UniqueIdentifier)));
    }

    [Test]
    public void DistinguishedName_IsReadOnlyInterpolationOfSamAccountNameAndContainer()
    {
        Assert.That(Bag.Property("DistinguishedName"), Is.EqualTo(new PropertyDefinition("DistinguishedName", PropertyType.Text)
        {
            ValueGenerator = "$ $'CN={Context.SamAccountName},{Services.GetProvisioningContainer(Context.Options.ProvisioningMapping.Values, 'AdminAdAccountContainer')}'",
            ReadOnly = true,
        }));
    }

    [Test]
    public void IdForTargetSystem_DependsOnDomainAndSamAccountName()
    {
        Assert.That(Bag.Property("IdForTargetSystem"), Is.EqualTo(new PropertyDefinition("IdForTargetSystem", PropertyType.Text)
        {
            ValueGenerator = "$Services.GetIdToCheck(Context.Domain.Key,Context.SamAccountName)",
            ReadOnly = true,
        }));
    }

    [Test]
    public void StartDate_IsPlainDateDespiteTrailingComma()
    {
        Assert.That(Bag.Property("StartDate"), Is.EqualTo(new PropertyDefinition("StartDate", PropertyType.Date)));
    }

    [Test]
    public void EndDate_MustBeInTheFuture()
    {
        Assert.That(Bag.Property("EndDate"), Is.EqualTo(new PropertyDefinition("EndDate", PropertyType.Date)
        {
            Validators = new PropertyValidators { FutureDate = true },
        }));
    }

    [TestCase("UPNOverride")]
    [TestCase("Comments")]
    [TestCase("Description")]
    public void FreeText_HasNoRules(string id)
    {
        Assert.That(Bag.Property(id), Is.EqualTo(new PropertyDefinition(id, PropertyType.Text)));
    }
}
