using MediatR;
using Microsoft.EntityFrameworkCore;
using PCShop.Application.Common.Exceptions;
using PCShop.Application.Common.Interfaces;
using PCShop.Application.Products.DTOs;

namespace PCShop.Application.Products.Queries.GetProductById
{
    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto>
    {
        private readonly IAppDbContext _context;

        public GetProductByIdQueryHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (product == null)
            {
                throw new NotFoundException($"Product with ID {request.Id} was not found.");
            }

            var now = DateTime.UtcNow;
            var discountPrice = product.DiscountPrice != null 
                && (product.DiscountStartDate == null || product.DiscountStartDate <= now) 
                && (product.DiscountEndDate == null || product.DiscountEndDate >= now) 
                ? product.DiscountPrice 
                : null;

            return new ProductDto(
                product.Id,
                product.Name,
                product.Brand,
                product.Price,
                discountPrice,
                product.ImageUrls.FirstOrDefault() ?? string.Empty,
                product.StockQuantity,
                product.Description,
                product.ImageUrls,
                product.Attributes,
                product.DiscountStartDate,
                product.DiscountEndDate,
                product.DiscountPrice
            );
        }
    }
}
