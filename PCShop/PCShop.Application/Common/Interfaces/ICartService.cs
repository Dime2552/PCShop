using PCShop.Application.Common.Models;

namespace PCShop.Application.Common.Interfaces
{
    public interface ICartService
    {
        Task<List<RedisCartItem>> GetCartAsync(string cartId);
        Task SetCartAsync(string cartId, List<RedisCartItem> items, TimeSpan? expiration = null);
        Task DeleteCartAsync(string cartId);
    }
}
