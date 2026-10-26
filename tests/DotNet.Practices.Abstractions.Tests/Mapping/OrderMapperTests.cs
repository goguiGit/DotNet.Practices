using DotNet.Practices.Abstractions.Mapping;
using DotNet.Practices.Abstractions.Mapping.Interfaces;
using DotNet.Practices.Abstractions.Mapping.Mappers;

namespace DotNet.Practices.Abstractions.Tests.Mapping;

[TestFixtureSource(nameof(Mappers))]
public class OrderMapperTests(IOrderMapper mapper)
{
    public static IEnumerable<TestFixtureData> Mappers =>
    [
        new TestFixtureData(new ManualOrderMapper()) { TestName = nameof(ManualOrderMapper) },
        new TestFixtureData(AutoMapperOrderMapper.CreateDefault()) { TestName = nameof(AutoMapperOrderMapper) },
    ];

    private static readonly Order SampleOrder = new(
        Id: Guid.Parse("11111111-1111-1111-1111-111111111111"),
        CustomerName: "Ada Lovelace",
        CreatedAt: new DateTimeOffset(2026, 10, 26, 15, 30, 0, TimeSpan.Zero),
        Lines:
        [
            new OrderLine("Keyboard", Quantity: 2, UnitPrice: 50m),
            new OrderLine("Mouse", Quantity: 1, UnitPrice: 25.5m),
        ]);

    [Test]
    public void ToDto_CopiesIdAndCustomerName()
    {
        var dto = mapper.ToDto(SampleOrder);

        Assert.Multiple(() =>
        {
            Assert.That(dto.Id, Is.EqualTo(SampleOrder.Id));
            Assert.That(dto.CustomerName, Is.EqualTo("Ada Lovelace"));
        });
    }

    [Test]
    public void ToDto_ComputesTotalAndLineCount()
    {
        var dto = mapper.ToDto(SampleOrder);

        Assert.Multiple(() =>
        {
            Assert.That(dto.Total, Is.EqualTo(125.5m));
            Assert.That(dto.LineCount, Is.EqualTo(2));
        });
    }

    [Test]
    public void ToDto_FormatsCreatedOnAsIsoDate()
    {
        var dto = mapper.ToDto(SampleOrder);

        Assert.That(dto.CreatedOn, Is.EqualTo("2026-10-26"));
    }

    [Test]
    public void ToDto_MapsLinesWithLineTotal()
    {
        var dto = mapper.ToDto(SampleOrder);

        Assert.That(dto.Lines, Is.EqualTo(new[]
        {
            new OrderLineDto("Keyboard", Quantity: 2, LineTotal: 100m),
            new OrderLineDto("Mouse", Quantity: 1, LineTotal: 25.5m),
        }));
    }

    [Test]
    public void ToDto_WithNullOrder_ThrowsArgumentNullException()
    {
        Assert.That(() => mapper.ToDto(null!), Throws.ArgumentNullException);
    }
}
