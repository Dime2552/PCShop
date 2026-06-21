using PCShop.Domain.Enums;
using PCShop.Domain.ValueObjects;

namespace PCShop.Domain.Entities
{
    public class Order
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User? User { get; set; }

        public decimal TotalAmount { get; set; }
        public decimal ShippingCost { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public Address ShippingAddress { get; set; } = new();

        // Integrations
        public string? StripeSessionId { get; set; }
        public string? TrackingNumber { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public List<OrderItem> Items { get; set; } = new();
    }
}
