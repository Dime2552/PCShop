using MediatR;

namespace PCShop.Application.Products.Commands.UpdateProductStockAndPrice
{
    public record UpdateProductStockAndPriceCommand(
        Guid Id,
        decimal Price,
        decimal? DiscountPrice,
        DateTime? DiscountStartDate,
        DateTime? DiscountEndDate,
        int StockQuantity
    ) : IRequest;
}
