using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PCShop.Application.Orders.Commands.CreateOrder;
using PCShop.Domain.ValueObjects;
using System.Security.Claims;

namespace PCShop.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateOrder([FromBody] CreateOrderRequest request)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            // At this point, the cart is strictly tied to the user's ID
            var cartId = $"cart:user:{userId}";

            var command = new CreateOrderCommand(
                cartId,
                userId,
                request.ShippingAddress,
                request.ShippingMethod
            );

            var orderId = await _mediator.Send(command);

            return Ok(orderId);
        }
    }

    public record CreateOrderRequest(Address ShippingAddress, string ShippingMethod);
}
