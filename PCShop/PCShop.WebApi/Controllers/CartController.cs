using MediatR;
using Microsoft.AspNetCore.Mvc;
using PCShop.Application.Cart.Commands;
using PCShop.Application.Cart.DTOs;
using PCShop.Application.Cart.Queries;

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

        private string GetSessionId()
        {
            if (Request.Headers.TryGetValue("x-session-id", out var sessionId))
                return $"cart:session:{sessionId}";

            throw new Exception("Session ID is missing");
        }

        [HttpGet]
        public async Task<ActionResult<CartResponseDto>> GetCart()
        {
            var result = await _mediator.Send(new GetCartQuery(GetSessionId()));
            return Ok(result);
        }

        [HttpPost("items")]
        public async Task<IActionResult> AddItem([FromBody] AddCartItemRequest request)
        {
            await _mediator.Send(new AddToCartCommand(GetSessionId(), request.ProductId, request.Quantity));
            return Ok();
        }

        [HttpPut("items/{productId}")]
        public async Task<IActionResult> UpdateQuantity(Guid productId, [FromBody] UpdateQuantityRequest request)
        {
            await _mediator.Send(new UpdateCartItemQuantityCommand(GetSessionId(), productId, request.Quantity));
            return Ok();
        }

        [HttpDelete("items/{productId}")]
        public async Task<IActionResult> RemoveItem(Guid productId)
        {
            await _mediator.Send(new RemoveFromCartCommand(GetSessionId(), productId));
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> ClearCart()
        {
            await _mediator.Send(new ClearCartCommand(GetSessionId()));
            return Ok();
        }
    }

    public record AddCartItemRequest(Guid ProductId, int Quantity);
    public record UpdateQuantityRequest(int Quantity);
}