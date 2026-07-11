namespace PCShop.Application.Orders.Commands.CreateOrder
{
    public record CreateOrderResponse(Guid OrderId, string CheckoutUrl);
}
