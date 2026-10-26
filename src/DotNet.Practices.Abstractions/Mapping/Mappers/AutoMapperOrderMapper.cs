using AutoMapper;
using DotNet.Practices.Abstractions.Mapping.Interfaces;

namespace DotNet.Practices.Abstractions.Mapping.Mappers;

public sealed class AutoMapperOrderMapper(IMapper mapper) : IOrderMapper
{
    public static AutoMapperOrderMapper CreateDefault() =>
        new(new MapperConfiguration(cfg => cfg.AddProfile<OrderProfile>()).CreateMapper());

    public OrderDto ToDto(Order order)
    {
        // IMapper.Map(null) silently returns null; keep the same contract as the manual mapper.
        ArgumentNullException.ThrowIfNull(order);
        return mapper.Map<OrderDto>(order);
    }
}
