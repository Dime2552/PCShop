using MediatR;
using Microsoft.EntityFrameworkCore;
using PCShop.Application.Common.Exceptions;
using PCShop.Application.Common.Interfaces;
using PCShop.Domain.Entities;
using PCShop.Domain.Enums;

namespace PCShop.Application.Orders.Commands.CreateOrder
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, CreateOrderResponse>
    {
        private readonly IAppDbContext _context;
        private readonly ICartService _cartService;
        private readonly IShippingService _shippingService;
        private readonly IPaymentService _paymentService;

        public CreateOrderCommandHandler(IAppDbContext context, ICartService cartService, IShippingService shippingService, IPaymentService paymentService)
        {
            _context = context;
            _cartService = cartService;
            _shippingService = shippingService;
            _paymentService = paymentService;
        }

        public async Task<CreateOrderResponse> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            // Get Cart from Redis
            var cartItems = await _cartService.GetCartAsync(request.CartId);

            if (!cartItems.Any())
                throw new BadRequestException("Cart is empty.");

            var productIds = cartItems.Select(c => c.ProductId).ToList();

            // Fetch actual products from MSSQL
            var products = await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id, cancellationToken);

            var order = new Order
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                ShippingAddress = request.ShippingAddress,
                Status = OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            decimal totalProductsAmount = 0;

            var now = DateTime.UtcNow;

            // Process each item
            foreach (var cartItem in cartItems)
            {
                if (!products.TryGetValue(cartItem.ProductId, out var product))
                    throw new BadRequestException($"Product with ID {cartItem.ProductId} not found.");

                if (product.StockQuantity < cartItem.Quantity)
                    throw new BadRequestException($"Not enough stock for product {product.Name}. Available: {product.StockQuantity}.");

                // Decrease stock quantity
                product.StockQuantity -= cartItem.Quantity;

                // Ignoring any prices passed from the client
                var isDiscountActive = product.DiscountPrice.HasValue &&
                                       (!product.DiscountStartDate.HasValue || product.DiscountStartDate <= now) &&
                                       (!product.DiscountEndDate.HasValue || product.DiscountEndDate >= now);
                var price = isDiscountActive ? product.DiscountPrice.Value : product.Price;

                order.Items.Add(new OrderItem
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    ProductId = product.Id,
                    Quantity = cartItem.Quantity,
                    UnitPrice = price
                });

                totalProductsAmount += price * cartItem.Quantity;
            }

            // Calculate shipping cost using Mock API
            var shippingCost = await _shippingService.CalculateShippingCostAsync(request.ShippingAddress, request.ShippingMethod, cancellationToken);

            order.ShippingCost = shippingCost;
            order.TotalAmount = totalProductsAmount + shippingCost;

            // Generate Stripe Checkout Session
            var (sessionId, checkoutUrl) = await _paymentService.CreateCheckoutSessionAsync(order, request.SuccessUrl, request.CancelUrl, cancellationToken);
            order.StripeSessionId = sessionId;

            _context.Orders.Add(order);

            // Save to database. 
            await _context.SaveChangesAsync(cancellationToken);

            // Clear Redis cart after successful order creation
            await _cartService.DeleteCartAsync(request.CartId);

            return new CreateOrderResponse(order.Id, checkoutUrl);
        }
    }
}
