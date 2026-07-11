using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PCShop.Application.Common.Interfaces;
using PCShop.Domain.Entities;
using PCShop.Infrastructure.Settings;
using Stripe.Checkout;

namespace PCShop.Infrastructure.Services
{
    public class StripePaymentService : IPaymentService
    {
        private readonly StripeSettings _stripeSettings;
        private readonly IAppDbContext _context;

        public StripePaymentService(IOptions<StripeSettings> stripeOptions, IAppDbContext context)
        {
            _stripeSettings = stripeOptions.Value;
            _context = context;
        }

        public async Task<(string SessionId, string CheckoutUrl)> CreateCheckoutSessionAsync(Order order, string successUrl, string cancelUrl, CancellationToken cancellationToken)
        {
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                Mode = "payment",
                SuccessUrl = successUrl,
                CancelUrl = cancelUrl,
                LineItems = new List<SessionLineItemOptions>(),
                ClientReferenceId = order.Id.ToString()
            };

            foreach (var item in order.Items)
            {
                // Ensure we have product info for the name
                var product = item.Product ?? await _context.Products.FindAsync(new object[] { item.ProductId }, cancellationToken);
                var productName = product?.Name ?? "Unknown Product";

                options.LineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.UnitPrice * 100), // Stripe expects amounts in cents
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = productName
                        }
                    },
                    Quantity = item.Quantity
                });
            }

            // Add shipping as a separate line item if > 0
            if (order.ShippingCost > 0)
            {
                options.LineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(order.ShippingCost * 100),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = "Shipping"
                        }
                    },
                    Quantity = 1
                });
            }

            var service = new SessionService();
            var session = await service.CreateAsync(options, cancellationToken: cancellationToken);

            return (session.Id, session.Url);
        }
    }
}
