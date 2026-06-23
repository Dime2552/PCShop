using MediatR;
using Microsoft.EntityFrameworkCore;
using PCShop.Application.Common.Interfaces;

namespace PCShop.Application.Cart.Commands
{
    public class UpdateCartItemQuantityCommandHandler : IRequestHandler<UpdateCartItemQuantityCommand, Unit>
    {
        private readonly ICartService _cartService;
        private readonly IAppDbContext _context;

        public UpdateCartItemQuantityCommandHandler(ICartService cartService, IAppDbContext context)
        {
            _cartService = cartService;
            _context = context;
        }

        public async Task<Unit> Handle(UpdateCartItemQuantityCommand request, CancellationToken cancellationToken)
        {
            if (request.Quantity <= 0)
            {
                // If count 0 or less, deleting product
                var cartItems = await _cartService.GetCartAsync(request.CartId);
                cartItems.RemoveAll(x => x.ProductId == request.ProductId);
                await _cartService.SetCartAsync(request.CartId, cartItems);
                return Unit.Value;
            }

            var product = await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken);

            if (product == null)
                throw new Exception("Product not found.");
            if (request.Quantity > product.StockQuantity)
                throw new Exception($"Only {product.StockQuantity} items in stock.");

            var items = await _cartService.GetCartAsync(request.CartId);
            var existingItem = items.FirstOrDefault(x => x.ProductId == request.ProductId);

            if (existingItem != null)
            {
                existingItem.Quantity = request.Quantity;
                await _cartService.SetCartAsync(request.CartId, items);
            }

            return Unit.Value;
        }
    }
}
