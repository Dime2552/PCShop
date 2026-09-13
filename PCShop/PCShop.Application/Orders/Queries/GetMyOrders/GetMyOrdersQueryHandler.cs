using MediatR;
using Microsoft.EntityFrameworkCore;
using PCShop.Application.Common.Interfaces;
using PCShop.Application.Orders.DTOs;

namespace PCShop.Application.Orders.Queries.GetMyOrders
{
    public class GetMyOrdersQueryHandler : IRequestHandler<GetMyOrdersQuery, List<OrderDto>>
    {
        private readonly IAppDbContext _context;

        public GetMyOrdersQueryHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<List<OrderDto>> Handle(GetMyOrdersQuery request, CancellationToken cancellationToken)
        {
            var orders = await _context.Orders
                .Where(o => o.UserId == request.UserId)
                .OrderByDescending(o => o.CreatedAt)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return orders.Select(o => new OrderDto(
                o.Id,
                o.TotalAmount,
                o.ShippingCost,
                o.Status.ToString(),
                o.CreatedAt,
                o.Items.Select(i => new OrderItemDto(
                    i.ProductId,
                    i.Product?.Name ?? string.Empty,
                    i.Quantity,
                    i.UnitPrice
                )).ToList()
            )).ToList();
        }
    }
}
