using MediatR;
using Microsoft.EntityFrameworkCore;
using PCShop.Application.Common.Exceptions;
using PCShop.Application.Common.Interfaces;
using PCShop.Application.Common.Models;

namespace PCShop.Application.Cart.Commands
{
    public class AddToCartCommandHandler : IRequestHandler<AddToCartCommand, Unit>
    {
        private readonly ICartService _cartService;
        private readonly IAppDbContext _context;

        public AddToCartCommandHandler(ICartService cartService, IAppDbContext context)
        {
            _cartService = cartService;
            _context = context;
        }

        public async Task<Unit> Handle(AddToCartCommand request, CancellationToken cancellationToken)
        {
            var product = await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken);

            if (product == null)
                throw new NotFoundException("Product not found.");

            var cartItems = await _cartService.GetCartAsync(request.CartId);
            var existingItem = cartItems.FirstOrDefault(x => x.ProductId == request.ProductId);

            if (existingItem != null)
            {
                if (existingItem.Quantity + request.Quantity > product.StockQuantity)
                    throw new BadRequestException("Not enough stock available.");

                existingItem.Quantity += request.Quantity;
            }
            else
            {
                if (request.Quantity > product.StockQuantity)
                    throw new BadRequestException("Not enough stock available.");

                cartItems.Add(new RedisCartItem { ProductId = request.ProductId, Quantity = request.Quantity });
            }

            await _cartService.SetCartAsync(request.CartId, cartItems);

            return Unit.Value;
        }
    }

}
