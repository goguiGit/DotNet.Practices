using DotNet.Practices.ScriptedMapping.Services;

namespace DotNet.Practices.ScriptedMapping.Tests.Services;

public class ScriptServicesTests
{
    private static ScriptServices NewServices() => new(new InMemorySequenceTracker());

    [Test]
    public void GetFiveTwoOneIdValue_FirstCall_IsFiveTwoAndCounterOne()
    {
        Assert.That(NewServices().GetFiveTwoOneIdValue("Lovelace", "Ada", false, "-adm-26"), Is.EqualTo("lovelad1-adm-26"));
    }

    [Test]
    public void GetFiveTwoOneIdValue_SecondCallForSamePerson_AdvancesTheCounter()
    {
        var services = NewServices();
        services.GetFiveTwoOneIdValue("Lovelace", "Ada", false, "-adm-26");

        Assert.That(services.GetFiveTwoOneIdValue("Lovelace", "Ada", false, "-adm-26"), Is.EqualTo("lovelad2-adm-26"));
    }

    [Test]
    public void GetFiveTwoOneIdValue_CounterTen_IsBase36UppercaseEvenWhenLower()
    {
        var services = NewServices();
        for (var i = 0; i < 9; i++)
        {
            services.GetFiveTwoOneIdValue("Lovelace", "Ada", false);
        }

        Assert.That(services.GetFiveTwoOneIdValue("Lovelace", "Ada", false), Is.EqualTo("loveladA"));
    }

    [Test]
    public void GetFiveTwoOneIdValue_CounterNeedsTwoDigits_SecondShrinksToOneCharacter()
    {
        var services = NewServices();
        for (var i = 0; i < 35; i++)
        {
            services.GetFiveTwoOneIdValue("Lovelace", "Ada", false);
        }

        Assert.That(services.GetFiveTwoOneIdValue("Lovelace", "Ada", false), Is.EqualTo("lovela10"));
    }

    [Test]
    public void GetFiveTwoOneIdValue_Upper_UppercasesTheNameParts()
    {
        Assert.That(NewServices().GetFiveTwoOneIdValue("Lovelace", "Ada", true), Is.EqualTo("LOVELAD1"));
    }

    [Test]
    public void GetFiveTwoOneIdValue_RemovesSpacesOnly()
    {
        Assert.That(NewServices().GetFiveTwoOneIdValue("De La Cruz", "Jo-An", false), Is.EqualTo("delacjo1"));
    }

    [Test]
    public void GetFiveTwoOneIdValue_ShortNames_AreUsedAsIs()
    {
        Assert.That(NewServices().GetFiveTwoOneIdValue("Li", "Bo", false), Is.EqualTo("libo1"));
    }

    [TestCase("", "Ada")]
    [TestCase("Lovelace", " ")]
    public void GetFiveTwoOneIdValue_EmptyPart_ThrowsArgumentException(string first, string second)
    {
        Assert.That(() => NewServices().GetFiveTwoOneIdValue(first, second, false), Throws.ArgumentException);
    }

    [Test]
    public void GetProvisioningContainer_ReturnsMappedValue()
    {
        var mapping = new Dictionary<string, string> { ["AdminAdAccountContainer"] = "OU=Admins,DC=qaacc,DC=int" };

        Assert.That(NewServices().GetProvisioningContainer(mapping, "AdminAdAccountContainer"), Is.EqualTo("OU=Admins,DC=qaacc,DC=int"));
    }

    [Test]
    public void GetProvisioningContainer_MissingKey_ReturnsEmpty()
    {
        Assert.That(NewServices().GetProvisioningContainer(new Dictionary<string, string>(), "Nope"), Is.Empty);
    }

    [Test]
    public void GetIdToCheck_JoinsDomainAndUserWithBackslash()
    {
        Assert.That(NewServices().GetIdToCheck("qaacc.int", "lovelad1-adm-26"), Is.EqualTo(@"qaacc.int\lovelad1-adm-26"));
    }

    [TestCase("", "user")]
    [TestCase("domain", "")]
    public void GetIdToCheck_EmptyPart_ReturnsEmpty(string domain, string user)
    {
        Assert.That(NewServices().GetIdToCheck(domain, user), Is.Empty);
    }

    [Test]
    public void GetNextIdValue_FormatsTheSequenceFromTheInitialValue()
    {
        var services = NewServices();

        Assert.That((services.GetNextIdValue("{0:00000}", 1), services.GetNextIdValue("{0:00000}", 1)), Is.EqualTo(("00001", "00002")));
    }

    [Test]
    public void GetNextIdValue_TemplateWithoutPlaceholder_ThrowsArgumentException()
    {
        Assert.That(() => NewServices().GetNextIdValue("fixed", 1), Throws.ArgumentException);
    }

    [Test]
    public void Combine_JoinsWithABackslash()
    {
        Assert.That(NewServices().Combine(@"\\files\home", "lovelad1"), Is.EqualTo(@"\\files\home\lovelad1"));
    }
}
