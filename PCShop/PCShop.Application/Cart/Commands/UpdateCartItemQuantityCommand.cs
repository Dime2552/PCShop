using MediatR;

namespace PCShop.Application.Cart.Commands
{
    public record UpdateCartItemQuantityCommand(string CartId, Guid ProductId, int Quantity) : IRequest<Unit>;
}
