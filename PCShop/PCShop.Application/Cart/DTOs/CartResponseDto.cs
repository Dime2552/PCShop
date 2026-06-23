namespace PCShop.Application.Cart.DTOs
{
    public record CartItemResponseDto(
        Guid ProductId,
        string Name,
        string MainImageUrl,
        decimal Price,
        int Quantity,
        decimal TotalPrice
    );

    public record CartResponseDto(
        string CartId,
        List<CartItemResponseDto> Items,
        decimal TotalCartPrice
    );
}
