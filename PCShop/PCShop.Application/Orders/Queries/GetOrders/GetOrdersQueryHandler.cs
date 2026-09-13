using MediatR;
using Microsoft.EntityFrameworkCore;
using PCShop.Application.Common.Interfaces;
using PCShop.Application.Orders.DTOs;

namespace PCShop.Application.Orders.Queries.GetOrders
{
    public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, List<AdminOrderDto>>
    {
        private readonly IAppDbContext _context;

        public GetOrdersQueryHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<List<AdminOrderDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Orders
                .Include(o => o.User)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .OrderByDescending(o => o.CreatedAt)
                .AsNoTracking();

            if (request.PageNumber.HasValue && request.PageSize.HasValue && request.PageNumber > 0 && request.PageSize > 0)
            {
                query = query.Skip((request.PageNumber.Value - 1) * request.PageSize.Value).Take(request.PageSize.Value);
            }

            var orders = await query.ToListAsync(cancellationToken);

            return orders.Select(o => new AdminOrderDto(
                o.Id,
                o.TotalAmount,
                o.ShippingCost,
                o.Status.ToString(),
                o.TrackingNumber,
                o.CreatedAt,
                o.UserId,
                o.User?.Email ?? string.Empty,
                $"{o.User?.FirstName} {o.User?.LastName}".Trim(),
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
