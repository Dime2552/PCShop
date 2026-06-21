using MediatR;
using PCShop.Application.Common.Interfaces;

namespace PCShop.Application.Products.Commands.CreateProduct
{
    public record CreateProductCommand(
    string Name,
    int CategoryId,
    string Brand,
    decimal Price,
    int StockQuantity,
    string? Description,
    List<FileUploadDto> Images,
    Dictionary<string, string> Attributes
) : IRequest<Guid>;
}
