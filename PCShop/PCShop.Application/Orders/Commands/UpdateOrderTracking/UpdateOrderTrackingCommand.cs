using MediatR;

namespace PCShop.Application.Orders.Commands.UpdateOrderTracking
{
    public record UpdateOrderTrackingCommand(Guid OrderId, string TrackingNumber) : IRequest;
}
