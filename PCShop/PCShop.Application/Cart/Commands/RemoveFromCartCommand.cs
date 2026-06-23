using MediatR;

namespace PCShop.Application.Cart.Commands
{
    public record RemoveFromCartCommand(string CartId, Guid ProductId) : IRequest<Unit>;
}
