namespace PCShop.Application.Reviews.DTOs
{
    public record ReviewDto(
        string AuthorName,
        int Rating,
        string Comment,
        DateTime CreatedAt,
        bool IsVerifiedPurchase = false);
}
