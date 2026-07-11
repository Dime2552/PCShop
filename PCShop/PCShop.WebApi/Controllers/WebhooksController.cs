using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PCShop.Application.Orders.Commands.UpdateOrderStatus;
using PCShop.Domain.Enums;
using PCShop.Infrastructure.Settings;
using Stripe;
using Stripe.Checkout;

namespace PCShop.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WebhooksController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly StripeSettings _stripeSettings;
        private readonly ILogger<WebhooksController> _logger;

        public WebhooksController(IMediator mediator, IOptions<StripeSettings> stripeSettings, ILogger<WebhooksController> logger)
        {
            _mediator = mediator;
            _stripeSettings = stripeSettings.Value;
            _logger = logger;
        }

        [HttpPost("stripe")]
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"],
                    _stripeSettings.WebhookSecret
                );

                // Handle the event
                if (stripeEvent.Type == EventTypes.CheckoutSessionCompleted)
                {
                    var session = stripeEvent.Data.Object as Session;

                    if (session != null && Guid.TryParse(session.ClientReferenceId, out var orderId))
                    {
                        var command = new UpdateOrderStatusCommand(orderId, OrderStatus.Paid);
                        await _mediator.Send(command);
                        
                        _logger.LogInformation("Order {OrderId} status updated to Paid.", orderId);
                    }
                }

                return Ok();
            }
            catch (StripeException e)
            {
                _logger.LogError(e, "Stripe webhook failed.");
                return BadRequest();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while processing Stripe webhook.");
                return StatusCode(500);
            }
        }
    }
}
