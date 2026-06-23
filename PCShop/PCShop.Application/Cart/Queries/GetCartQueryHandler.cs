using MediatR;
using Microsoft.EntityFrameworkCore;
using PCShop.Application.Cart.DTOs;
using PCShop.Application.Common.Interfaces;

namespace PCShop.Application.Cart.Queries
{
    public class GetCartQueryHandler : IRequestHandler<GetCartQuery, CartResponseDto>
    {
        private readonly ICartService _cartService;
        private readonly IAppDbContext _context;

        public GetCartQueryHandler(ICartService cartService, IAppDbContext context)
        {
            _cartService = cartService;
            _context = context;
        }

        public async Task<CartResponseDto> Handle(GetCartQuery request, CancellationToken cancellationToken)
        {
            var redisItems = await _cartService.GetCartAsync(request.CartId);

            if (!redisItems.Any())
                return new CartResponseDto(request.CartId, new(), 0);

            var productIds = redisItems.Select(x => x.ProductId).ToList();

            // Get actual prices and data from MSSQL
            var products = await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id, cancellationToken);

            var responseItems = new List<CartItemResponseDto>();

            foreach (var item in redisItems)
            {
                if (products.TryGetValue(item.ProductId, out var product))
                {
                    var price = product.DiscountPrice ?? product.Price;
                    responseItems.Add(new CartItemResponseDto(
                        product.Id,
                        product.Name,
                        product.ImageUrls.FirstOrDefault() ?? string.Empty,
                        price,
                        item.Quantity,
                        price * item.Quantity
                    ));
                }
            }

            var totalCartPrice = responseItems.Sum(x => x.TotalPrice);

            return new CartResponseDto(request.CartId, responseItems, totalCartPrice);
        }
    }
}
