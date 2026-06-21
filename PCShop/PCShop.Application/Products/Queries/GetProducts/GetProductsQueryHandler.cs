using MediatR;
using Microsoft.EntityFrameworkCore;
using PCShop.Application.Common.Interfaces;
using PCShop.Application.Common.Models;
using PCShop.Application.Products.DTOs;

namespace PCShop.Application.Products.Queries.GetProducts
{
    public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, PaginatedList<ProductDto>>
    {
        private readonly IAppDbContext _context;

        public GetProductsQueryHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedList<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Products.Where(p => p.CategoryId == request.CategoryId).AsQueryable();

            // Dynamic filtering by JSON-column Attributes
            if (request.Filters != null && request.Filters.Any())
            {
                foreach (var filter in request.Filters)
                {
                    query = query
                        .AsNoTracking()
                        .Where(p => p.Attributes.Any(a => a.Key == filter.Key && a.Value == filter.Value));
                }
            }

            // Sorting
            query = request.SortBy switch
            {
                "price_asc" => query.OrderBy(p => p.DiscountPrice ?? p.Price),
                "price_desc" => query.OrderByDescending(p => p.DiscountPrice ?? p.Price),
                _ => query.OrderBy(p => p.Name)
            };

            var projectedQuery = query.Select(p => new ProductDto(
                p.Id,
                p.Name,
                p.Brand,
                p.Price,
                p.DiscountPrice,
                p.ImageUrls.FirstOrDefault() ?? string.Empty,
                p.StockQuantity
            ));

            return await PaginatedList<ProductDto>.CreateAsync(projectedQuery, request.PageNumber, request.PageSize);
        }
    }
}
