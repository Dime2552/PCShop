namespace PCShop.Application.Common.Models
{
    public class RedisCartItem
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
