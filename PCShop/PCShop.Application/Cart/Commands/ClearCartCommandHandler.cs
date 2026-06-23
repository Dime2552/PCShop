using MediatR;
using PCShop.Application.Common.Interfaces;

namespace PCShop.Application.Cart.Commands
{
    public class ClearCartCommandHandler : IRequestHandler<ClearCartCommand, Unit>
    {
        private readonly ICartService _cartService;

        public ClearCartCommandHandler(ICartService cartService) => _cartService = cartService;

        public async Task<Unit> Handle(ClearCartCommand request, CancellationToken cancellationToken)
        {
            await _cartService.DeleteCartAsync(request.CartId);
            return Unit.Value;
        }
    }

}
