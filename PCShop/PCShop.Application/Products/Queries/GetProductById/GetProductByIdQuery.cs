using MediatR;
using PCShop.Application.Products.DTOs;

namespace PCShop.Application.Products.Queries.GetProductById
{
    public record GetProductByIdQuery(Guid Id) : IRequest<ProductDto>;
}
