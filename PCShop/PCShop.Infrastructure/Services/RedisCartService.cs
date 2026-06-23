using PCShop.Application.Common.Interfaces;
using PCShop.Application.Common.Models;
using StackExchange.Redis;
using System.Text.Json;

namespace PCShop.Infrastructure.Services
{
    public class RedisCartService : ICartService
    {
        private readonly IDatabase _database;

        public RedisCartService(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }

        public async Task<List<RedisCartItem>> GetCartAsync(string cartId)
        {
            var data = await _database.StringGetAsync(cartId);

            if (data.IsNullOrEmpty)
            {
                return new List<RedisCartItem>();
            }

            return JsonSerializer.Deserialize<List<RedisCartItem>>(data.ToString()) ?? new List<RedisCartItem>();
        }

        public async Task SetCartAsync(string cartId, List<RedisCartItem> items, TimeSpan? expiration = null)
        {
            var data = JsonSerializer.Serialize(items);
            await _database.StringSetAsync(cartId, data, expiration ?? TimeSpan.FromDays(7));
        }

        public async Task DeleteCartAsync(string cartId)
        {
            await _database.KeyDeleteAsync(cartId);
        }
    }
}
