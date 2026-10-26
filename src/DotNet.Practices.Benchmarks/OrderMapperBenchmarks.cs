using BenchmarkDotNet.Attributes;
using DotNet.Practices.Abstractions.Mapping;
using DotNet.Practices.Abstractions.Mapping.Interfaces;
using DotNet.Practices.Abstractions.Mapping.Mappers;

namespace DotNet.Practices.Benchmarks;

[MemoryDiagnoser]
[ShortRunJob]
public class OrderMapperBenchmarks
{
    private readonly IOrderMapper _manual = new ManualOrderMapper();
    private readonly IOrderMapper _autoMapper = AutoMapperOrderMapper.CreateDefault();
    private Order _order = null!;

    [Params(1, 10, 100)]
    public int Lines { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var lines = Enumerable.Range(1, Lines)
            .Select(i => new OrderLine($"Product {i}", Quantity: i, UnitPrice: 9.99m))
            .ToList();
        _order = new Order(Guid.NewGuid(), "Ada Lovelace", DateTimeOffset.UtcNow, lines);
    }

    [Benchmark(Baseline = true)]
    public OrderDto Manual() => _manual.ToDto(_order);

    [Benchmark]
    public OrderDto AutoMapper() => _autoMapper.ToDto(_order);
}
