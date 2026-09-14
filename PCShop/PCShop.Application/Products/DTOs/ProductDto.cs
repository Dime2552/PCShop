using PCShop.Domain.Entities;

namespace PCShop.Application.Products.DTOs
{
    public record ProductDto(
        Guid Id,
        string Name,
        string Brand,
        decimal Price,
        decimal? DiscountPrice,
        string MainImageUrl,
        int StockQuantity,
        string? Description = null,
        List<string>? ImageUrls = null,
        List<ProductAttributeItem>? Attributes = null,
        DateTime? DiscountStartDate = null,
        DateTime? DiscountEndDate = null,
        decimal? RawDiscountPrice = null);
}
