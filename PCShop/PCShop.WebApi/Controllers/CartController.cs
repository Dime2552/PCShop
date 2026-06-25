using MediatR;
using Microsoft.AspNetCore.Mvc;
using PCShop.Application.Cart.Commands;
using PCShop.Application.Cart.DTOs;
using PCShop.Application.Cart.Queries;
using System.Security.Claims;

namespace PCShop.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CartController(IMediator mediator)
        {
            _mediator = mediator;
        }

        private string GetCartId()
        {
            // Check if user is authenticated
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userId))
            {
                return $"cart:user:{userId}";
            }

            // Fallback to guest session
            if (Request.Headers.TryGetValue("x-session-id", out var sessionId))
            {
                return $"cart:session:{sessionId}";
            }

            throw new Exception("Session ID is missing");
        }

        [HttpGet]
        public async Task<ActionResult<CartResponseDto>> GetCart()
        {
            var result = await _mediator.Send(new GetCartQuery(GetCartId()));
            return Ok(result);
        }

        [HttpPost("items")]
        public async Task<IActionResult> AddItem([FromBody] AddCartItemRequest request)
        {
            await _mediator.Send(new AddToCartCommand(GetCartId(), request.ProductId, request.Quantity));
            return Ok();
        }

        [HttpPut("items/{productId}")]
        public async Task<IActionResult> UpdateQuantity(Guid productId, [FromBody] UpdateQuantityRequest request)
        {
            await _mediator.Send(new UpdateCartItemQuantityCommand(GetCartId(), productId, request.Quantity));
            return Ok();
        }

        [HttpDelete("items/{productId}")]
        public async Task<IActionResult> RemoveItem(Guid productId)
        {
            await _mediator.Send(new RemoveFromCartCommand(GetCartId(), productId));
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> ClearCart()
        {
            await _mediator.Send(new ClearCartCommand(GetCartId()));
            return Ok();
        }
    }

    public record AddCartItemRequest(Guid ProductId, int Quantity);
    public record UpdateQuantityRequest(int Quantity);
}