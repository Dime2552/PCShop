using MediatR;
using PCShop.Application.Orders.DTOs;

namespace PCShop.Application.Orders.Queries.GetOrders
{
    public record GetOrdersQuery(int? PageNumber = null, int? PageSize = null) : IRequest<List<AdminOrderDto>>;
}
