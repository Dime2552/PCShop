using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PCShop.Application.Orders.Commands.CreateOrder;
using PCShop.Application.Orders.Commands.UpdateOrderTracking;
using PCShop.Application.Orders.DTOs;
using PCShop.Application.Orders.Queries.GetMyOrders;
using PCShop.Application.Orders.Queries.GetOrders;
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
        private readonly IConfiguration _configuration;

        public OrdersController(IMediator mediator, IConfiguration configuration)
        {
            _mediator = mediator;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<ActionResult<CreateOrderResponse>> CreateOrder([FromBody] CreateOrderRequest request)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            // At this point, the cart is strictly tied to the user's ID
            var cartId = $"cart:user:{userId}";

            var clientUrl = _configuration["ClientUrl"] ?? "http://localhost:4200";

            var command = new CreateOrderCommand(
                cartId,
                userId,
                request.ShippingAddress,
                request.ShippingMethod,
                $"{clientUrl}/checkout/success",
                $"{clientUrl}/checkout/cancel"
            );

            var response = await _mediator.Send(command);

            return Ok(response);
        }

        [HttpGet]
        public async Task<ActionResult<List<OrderDto>>> GetMyOrders()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var orders = await _mediator.Send(new GetMyOrdersQuery(userId));

            return Ok(orders);
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<AdminOrderDto>>> GetAllOrders([FromQuery] int? pageNumber, [FromQuery] int? pageSize)
        {
            var orders = await _mediator.Send(new GetOrdersQuery(pageNumber, pageSize));
            return Ok(orders);
        }

        [HttpPatch("{id:guid}/tracking")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateOrderTracking([FromRoute] Guid id, [FromBody] UpdateOrderTrackingRequest request)
        {
            await _mediator.Send(new UpdateOrderTrackingCommand(id, request.TrackingNumber));
            return Ok(new { message = "Tracking number updated successfully" });
        }
    }

    public record CreateOrderRequest(Address ShippingAddress, string ShippingMethod);
    public record UpdateOrderTrackingRequest(string TrackingNumber);
}
