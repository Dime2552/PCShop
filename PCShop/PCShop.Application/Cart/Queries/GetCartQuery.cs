using MediatR;
using PCShop.Application.Cart.DTOs;

namespace PCShop.Application.Cart.Queries
{
    public record GetCartQuery(string CartId) : IRequest<CartResponseDto>;
}
