using MediatR;
using PCShop.Domain.Enums;

namespace PCShop.Application.Orders.Commands.UpdateOrderStatus
{
    public record UpdateOrderStatusCommand(Guid OrderId, OrderStatus Status) : IRequest;
}
