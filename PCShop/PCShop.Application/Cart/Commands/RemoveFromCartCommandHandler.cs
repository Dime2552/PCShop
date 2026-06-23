using MediatR;
using PCShop.Application.Common.Interfaces;

namespace PCShop.Application.Cart.Commands
{
    public class RemoveFromCartCommandHandler : IRequestHandler<RemoveFromCartCommand, Unit>
    {
        private readonly ICartService _cartService;

        public RemoveFromCartCommandHandler(ICartService cartService)
        {
            _cartService = cartService;
        }

        public async Task<Unit> Handle(RemoveFromCartCommand request, CancellationToken cancellationToken)
        {
            var cartItems = await _cartService.GetCartAsync(request.CartId);

            var existingItem = cartItems.FirstOrDefault(x => x.ProductId == request.ProductId);
            if (existingItem != null)
            {
                cartItems.Remove(existingItem);
                await _cartService.SetCartAsync(request.CartId, cartItems);
            }

            return Unit.Value;
        }
    }
}
