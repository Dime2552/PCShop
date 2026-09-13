using MediatR;
using PCShop.Application.Reviews.DTOs;

namespace PCShop.Application.Reviews.Queries.GetProductReviews
{
    public record GetProductReviewsQuery(Guid ProductId) : IRequest<List<ReviewDto>>;
}
