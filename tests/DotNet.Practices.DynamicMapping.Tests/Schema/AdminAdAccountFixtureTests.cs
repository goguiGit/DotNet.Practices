using DotNet.Practices.DynamicMapping.Models;
using DotNet.Practices.DynamicMapping.Tests.Fixtures;
using DotNet.Practices.DynamicMapping.Validators;

namespace DotNet.Practices.DynamicMapping.Tests.Schema;

public class AdminAdAccountFixtureTests
{
    private static readonly PropertyBag Bag = PropertyBagFixtures.Read("AdminAdAccount.propertybags.json").Single();

    [Test]
    public void Bag_IsAdminAdAccountDataWithElevenPropertiesAndNoBase()
    {
        Assert.Multiple(() =>
        {
            Assert.That(Bag.Id, Is.EqualTo("AdminAdAccountData"));
            Assert.That(Bag.Properties, Has.Count.EqualTo(11));
            Assert.That(Bag.Extends, Is.Empty);
            Assert.That(Bag.Properties.Select(property => property.Type), Is.All.Not.Null);
        });
    }

    [Test]
    public void Domain_IsRequiredLookup()
    {
        var domain = Bag.Property("Domain");

        Assert.Multiple(() =>
        {
            Assert.That(domain.Type, Is.EqualTo(PropertyType.Lookup));
            Assert.That(domain.Lookup, Is.EqualTo("Lookup.AdAccountDomain"));
            Assert.That(domain.Validators.Required, Is.True);
        });
    }

    [Test]
    public void SamAccountName_IsUniqueTextWithDefaultFromOwnerAndStrictValidators()
    {
        var samAccountName = Bag.Property("SamAccountName");

        Assert.Multiple(() =>
        {
            Assert.That(samAccountName.Type, Is.EqualTo(PropertyType.Text));
            Assert.That(samAccountName.Unique, Is.EqualTo("ADAccountData"));
            Assert.That(samAccountName.DefaultValue, Is.EqualTo("$Services.GetFiveTwoOneIdValue(Context.Owner.Lastname, Context.Owner.Firstname, false, '-adm-26')"));
            Assert.That(samAccountName.Validators.Required, Is.True);
            Assert.That(samAccountName.Validators.MaxLength, Is.EqualTo(20));
            Assert.That(samAccountName.Validators.Pattern, Is.EqualTo(@"^[^\[\],:;\|=@\+\*\?<>/\\]+$"));
        });
    }

    [Test]
    public void DisplayName_IsReadOnlyCopyOfSamAccountName()
    {
        var displayName = Bag.Property("DisplayName");

        Assert.Multiple(() =>
        {
            Assert.That(displayName.ValueGenerator, Is.EqualTo("$Context.SamAccountName"));
            Assert.That(displayName.ReadOnly, Is.True, "file uses lowercase 'readonly'");
        });
    }

    [Test]
    public void AdGuid_IsUniqueIdentifierWithPlainStringName()
    {
        Assert.That(Bag.Property("AdGuid").Type, Is.EqualTo(PropertyType.UniqueIdentifier));
    }

    [Test]
    public void DistinguishedName_IsReadOnlyInterpolationOfSamAccountNameAndContainer()
    {
        var distinguishedName = Bag.Property("DistinguishedName");

        Assert.Multiple(() =>
        {
            Assert.That(distinguishedName.ReadOnly, Is.True);
            Assert.That(distinguishedName.ValueGenerator, Does.StartWith("$ $'CN={Context.SamAccountName},"));
            Assert.That(distinguishedName.ValueGenerator, Does.Contain("Services.GetProvisioningContainer(Context.Options.ProvisioningMapping.Values, 'AdminAdAccountContainer')"));
        });
    }

    [Test]
    public void IdForTargetSystem_DependsOnDomainAndSamAccountName()
    {
        var idForTargetSystem = Bag.Property("IdForTargetSystem");

        Assert.Multiple(() =>
        {
            Assert.That(idForTargetSystem.ReadOnly, Is.True);
            Assert.That(idForTargetSystem.ValueGenerator, Is.EqualTo("$Services.GetIdToCheck(Context.Domain.Key,Context.SamAccountName)"));
        });
    }

    [Test]
    public void Dates_StartDateSurvivesTrailingCommaAndEndDateMustBeInTheFuture()
    {
        Assert.Multiple(() =>
        {
            Assert.That(Bag.Property("StartDate").Type, Is.EqualTo(PropertyType.Date));
            Assert.That(Bag.Property("StartDate").Validators, Is.EqualTo(PropertyValidators.None));
            Assert.That(Bag.Property("EndDate").Validators.FutureDate, Is.True);
        });
    }

    [Test]
    public void FreeTextProperties_HaveNoRulesAtAll()
    {
        foreach (var id in new[] { "UPNOverride", "Comments", "Description" })
        {
            var property = Bag.Property(id);
            Assert.Multiple(() =>
            {
                Assert.That(property.Type, Is.EqualTo(PropertyType.Text), id);
                Assert.That(property.DefaultValue, Is.Null, id);
                Assert.That(property.ValueGenerator, Is.Null, id);
                Assert.That(property.Validators, Is.EqualTo(PropertyValidators.None), id);
            });
        }
    }
}
