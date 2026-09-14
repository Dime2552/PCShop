using MediatR;
using Microsoft.EntityFrameworkCore;
using PCShop.Application.Common.Exceptions;
using PCShop.Application.Common.Interfaces;

namespace PCShop.Application.Products.Commands.UpdateProductStockAndPrice
{
    public class UpdateProductStockAndPriceCommandHandler : IRequestHandler<UpdateProductStockAndPriceCommand>
    {
        private readonly IAppDbContext _context;

        public UpdateProductStockAndPriceCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task Handle(UpdateProductStockAndPriceCommand request, CancellationToken cancellationToken)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (product == null)
            {
                throw new NotFoundException($"Product with ID {request.Id} was not found.");
            }

            product.Price = request.Price;
            product.DiscountPrice = request.DiscountPrice;
            product.DiscountStartDate = request.DiscountPrice.HasValue ? request.DiscountStartDate : null;
            product.DiscountEndDate = request.DiscountPrice.HasValue ? request.DiscountEndDate : null;
            product.StockQuantity = request.StockQuantity;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
