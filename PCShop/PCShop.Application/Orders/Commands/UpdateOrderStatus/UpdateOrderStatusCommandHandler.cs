using MediatR;
using PCShop.Application.Common.Exceptions;
using PCShop.Application.Common.Interfaces;

namespace PCShop.Application.Orders.Commands.UpdateOrderStatus
{
    public class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand>
    {
        private readonly IAppDbContext _context;

        public UpdateOrderStatusCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
        {
            var order = await _context.Orders.FindAsync(new object[] { request.OrderId }, cancellationToken);

            if (order == null)
            {
                throw new NotFoundException($"Order with id {request.OrderId} was not found.");
            }

            order.Status = request.Status;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
