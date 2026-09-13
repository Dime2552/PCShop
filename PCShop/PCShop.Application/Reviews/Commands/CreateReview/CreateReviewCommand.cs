using MediatR;

namespace PCShop.Application.Reviews.Commands.CreateReview
{
    public record CreateReviewCommand(
        Guid ProductId,
        Guid UserId,
        int Rating,
        string Comment) : IRequest<Guid>;
}
