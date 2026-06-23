namespace PCShop.Application.Products.DTOs
{
    public record ProductDto(
        Guid Id,
        string Name,
        string Brand,
        decimal Price,
        decimal? DiscountPrice,
        string MainImageUrl,
        int StockQuantity);
}
