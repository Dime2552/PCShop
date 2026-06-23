using MediatR;

namespace PCShop.Application.Cart.Commands
{
    public record ClearCartCommand(string CartId) : IRequest<Unit>;
}
