using AutoMapper;

namespace DotNet.Practices.Abstractions.Mapping;

public sealed class OrderProfile : Profile
{
    public OrderProfile()
    {
        CreateMap<OrderLine, OrderLineDto>()
            .ForCtorParam(nameof(OrderLineDto.LineTotal), options => options.MapFrom(line => line.Quantity * line.UnitPrice));

        CreateMap<Order, OrderDto>()
            .ForCtorParam(nameof(OrderDto.CreatedOn), options => options.MapFrom(order => order.CreatedAt.ToString("yyyy-MM-dd")))
            .ForCtorParam(nameof(OrderDto.LineCount), options => options.MapFrom(order => order.Lines.Count))
            .ForCtorParam(nameof(OrderDto.Total), options => options.MapFrom(order => order.Lines.Sum(line => line.Quantity * line.UnitPrice)));
    }
}
