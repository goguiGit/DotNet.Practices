using AutoMapper;
using DotNet.Practices.Abstractions.Mapping;

namespace DotNet.Practices.Abstractions.Tests.Mapping;

public class OrderProfileTests
{
    [Test]
    public void Configuration_IsValid()
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<OrderProfile>());

        Assert.That(configuration.AssertConfigurationIsValid, Throws.Nothing);
    }
}
