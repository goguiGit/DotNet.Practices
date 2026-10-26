namespace DotNet.Practices.Abstractions.Mapping.Interfaces;

public interface IOrderMapper
{
    OrderDto ToDto(Order order);
}
