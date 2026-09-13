using MediatR;
using Microsoft.EntityFrameworkCore;
using PCShop.Application.Common.Exceptions;
using PCShop.Application.Common.Interfaces;
using PCShop.Domain.Entities;
using PCShop.Domain.Enums;

namespace PCShop.Application.Reviews.Commands.CreateReview
{
    public class CreateReviewCommandHandler : IRequestHandler<CreateReviewCommand, Guid>
    {
        private readonly IAppDbContext _context;

        public CreateReviewCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
        {
            var productExists = await _context.Products
                .AnyAsync(p => p.Id == request.ProductId, cancellationToken);

            if (!productExists)
            {
                throw new NotFoundException($"Product with ID {request.ProductId} was not found.");
            }

            var review = new Review
            {
                Id = Guid.NewGuid(),
                ProductId = request.ProductId,
                UserId = request.UserId,
                Rating = request.Rating,
                Comment = request.Comment,
                CreatedAt = DateTime.UtcNow
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync(cancellationToken);

            return review.Id;
        }
    }
}
