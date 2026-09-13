using MediatR;
using Microsoft.EntityFrameworkCore;
using PCShop.Application.Common.Exceptions;
using PCShop.Application.Common.Interfaces;
using PCShop.Domain.Enums;

namespace PCShop.Application.Orders.Commands.UpdateOrderTracking
{
    public class UpdateOrderTrackingCommandHandler : IRequestHandler<UpdateOrderTrackingCommand>
    {
        private readonly IAppDbContext _context;

        public UpdateOrderTrackingCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task Handle(UpdateOrderTrackingCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.TrackingNumber))
            {
                throw new BadRequestException("Tracking number is required.");
            }

            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

            if (order == null)
            {
                throw new NotFoundException($"Order with id {request.OrderId} was not found.");
            }

            if (order.Status != OrderStatus.Paid)
            {
                throw new BadRequestException($"Only paid orders can be shipped. Current order status is {order.Status}.");
            }

            order.TrackingNumber = request.TrackingNumber.Trim();
            order.Status = OrderStatus.Shipped;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
