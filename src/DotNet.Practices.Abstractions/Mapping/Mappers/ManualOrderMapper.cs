using DotNet.Practices.Abstractions.Mapping.Interfaces;

namespace DotNet.Practices.Abstractions.Mapping.Mappers;

public sealed class ManualOrderMapper : IOrderMapper
{
    public OrderDto ToDto(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);

        var lines = new OrderLineDto[order.Lines.Count];
        var total = 0m;
        for (var i = 0; i < lines.Length; i++)
        {
            var line = order.Lines[i];
            var lineTotal = line.Quantity * line.UnitPrice;
            lines[i] = new OrderLineDto(line.Product, line.Quantity, lineTotal);
            total += lineTotal;
        }

        return new OrderDto(
            order.Id,
            order.CustomerName,
            order.CreatedAt.ToString("yyyy-MM-dd"),
            lines.Length,
            total,
            lines);
    }
}
