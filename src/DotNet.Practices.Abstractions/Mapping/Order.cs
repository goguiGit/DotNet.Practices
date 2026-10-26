namespace DotNet.Practices.Abstractions.Mapping;

public record Order(Guid Id, string CustomerName, DateTimeOffset CreatedAt, IReadOnlyList<OrderLine> Lines);

public record OrderLine(string Product, int Quantity, decimal UnitPrice);
