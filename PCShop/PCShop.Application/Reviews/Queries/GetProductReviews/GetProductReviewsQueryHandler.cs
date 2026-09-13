using MediatR;
using Microsoft.EntityFrameworkCore;
using PCShop.Application.Common.Interfaces;
using PCShop.Application.Reviews.DTOs;
using PCShop.Domain.Enums;

namespace PCShop.Application.Reviews.Queries.GetProductReviews
{
    public class GetProductReviewsQueryHandler : IRequestHandler<GetProductReviewsQuery, List<ReviewDto>>
    {
        private readonly IAppDbContext _context;

        public GetProductReviewsQueryHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ReviewDto>> Handle(GetProductReviewsQuery request, CancellationToken cancellationToken)
        {
            var rawReviews = await _context.Reviews
                .AsNoTracking()
                .Where(r => r.ProductId == request.ProductId)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new
                {
                    r.UserId,
                    AuthorName = r.User != null ? r.User.FirstName : "Anonymous",
                    r.Rating,
                    Comment = r.Comment ?? string.Empty,
                    r.CreatedAt
                })
                .ToListAsync(cancellationToken);

            if (rawReviews.Count == 0)
            {
                return new List<ReviewDto>();
            }

            var userIds = rawReviews.Select(r => r.UserId).Distinct().ToList();

            var verifiedUserIds = await _context.Orders
                .AsNoTracking()
                .Where(o => userIds.Contains(o.UserId)
                    && (o.Status == OrderStatus.Paid || o.Status == OrderStatus.Shipped)
                    && o.Items.Any(i => i.ProductId == request.ProductId))
                .Select(o => o.UserId)
                .Distinct()
                .ToListAsync(cancellationToken);

            var verifiedUserSet = new HashSet<Guid>(verifiedUserIds);

            return rawReviews.Select(r => new ReviewDto(
                r.AuthorName,
                r.Rating,
                r.Comment,
                r.CreatedAt,
                verifiedUserSet.Contains(r.UserId)
            )).ToList();
        }
    }
}
