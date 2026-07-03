using MediatR;
using PCShop.Domain.ValueObjects;

namespace PCShop.Application.Orders.Commands.CreateOrder
{
    public record CreateOrderCommand(
        string CartId,
        Guid UserId,
        Address ShippingAddress,
        string ShippingMethod
    ) : IRequest<Guid>;
}
