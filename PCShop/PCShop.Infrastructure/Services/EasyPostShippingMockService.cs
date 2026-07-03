using PCShop.Application.Common.Interfaces;
using PCShop.Domain.ValueObjects;

namespace PCShop.Infrastructure.Services
{
    public class EasyPostShippingMockService : IShippingService
    {
        public Task<decimal> CalculateShippingCostAsync(Address address, string shippingMethod, CancellationToken cancellationToken)
        {
            decimal cost = shippingMethod.ToLower() switch
            {
                "express" => 25.00m,
                "standard" => 10.00m,
                _ => 10.00m
            };

            return Task.FromResult(cost);
        }
    }
}
