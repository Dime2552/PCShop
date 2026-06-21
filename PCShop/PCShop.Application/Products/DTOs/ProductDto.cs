using System;
using System.Collections.Generic;
using System.Text;

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
