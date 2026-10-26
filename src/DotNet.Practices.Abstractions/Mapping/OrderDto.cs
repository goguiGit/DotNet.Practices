namespace DotNet.Practices.Abstractions.Mapping;

public record OrderDto(
    Guid Id,
    string CustomerName,
    string CreatedOn,
    int LineCount,
    decimal Total,
    IReadOnlyList<OrderLineDto> Lines);

public record OrderLineDto(string Product, int Quantity, decimal LineTotal);
