using DotNet.Practices.DynamicMapping.Resolution;

namespace DotNet.Practices.DynamicMapping.Tests.Fixtures;

public static class SampleContext
{
    public static MappingContext AdaLovelace => new(
        Owner: new Dictionary<string, string>
        {
            ["Firstname"] = "Ada",
            ["Lastname"] = "Lovelace",
            ["Email"] = "ada.lovelace@qaacc.int",
            ["Department"] = "Research",
        },
        Inputs: new Dictionary<string, string>
        {
            ["Name"] = "Research Team",
            ["PrimarySmtpAddressLocalPart"] = "research-team",
            ["Description"] = "Distribution list and shared folder of the Research department",
        },
        Options: new Dictionary<string, string>
        {
            ["ProvisioningMapping.AdminAdAccountContainer"] = "OU=Admins,DC=qaacc,DC=int",
            ["ProvisioningMapping.StandardAdAccountContainer"] = "OU=Users,DC=qaacc,DC=int",
            ["ProvisioningMapping.SharedFolderAccessTypeGroupContainer"] = "OU=Groups,DC=qaacc,DC=int",
            ["ProvisioningMapping.DistributionGroupContainer"] = "OU=Groups,DC=qaacc,DC=int",
            ["ProvisioningMapping.DistributionGroupAccessTypeGroupContainer"] = "OU=Groups,DC=qaacc,DC=int",
            ["Lookup.AdAccountDomain"] = "qaacc.int, qaacc.test",
            ["Lookup.HomeDirectory"] = @"\\files\home, \\files\home2",
            ["Lookup.ShareRoot"] = @"\\files\shares",
            ["Lookup.EmailDomain"] = "qaacc.int",
        });
}
