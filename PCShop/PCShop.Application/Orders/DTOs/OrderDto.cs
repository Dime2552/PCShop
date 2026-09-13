namespace PCShop.Application.Orders.DTOs
{
    public record OrderItemDto(
        Guid ProductId,
        string ProductName,
        int Quantity,
        decimal UnitPrice
    );

    public record OrderDto(
        Guid OrderId,
        decimal TotalAmount,
        decimal ShippingCost,
        string Status,
        DateTime CreatedAt,
        List<OrderItemDto> Items
    );

    public record AdminOrderDto(
        Guid OrderId,
        decimal TotalAmount,
        decimal ShippingCost,
        string Status,
        string? TrackingNumber,
        DateTime CreatedAt,
        Guid UserId,
        string CustomerEmail,
        string CustomerName,
        List<OrderItemDto> Items
    );
}
