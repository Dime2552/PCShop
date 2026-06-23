using MediatR;

namespace PCShop.Application.Cart.Commands
{
    public record AddToCartCommand(string CartId, Guid ProductId, int Quantity) : IRequest<Unit>;
}
