using MediatR;
using PCShop.Application.Orders.DTOs;

namespace PCShop.Application.Orders.Queries.GetMyOrders
{
    public record GetMyOrdersQuery(Guid UserId) : IRequest<List<OrderDto>>;
}
