using PCShop.Domain.ValueObjects;

namespace PCShop.Application.Common.Interfaces
{
    public interface IShippingService
    {
        Task<decimal> CalculateShippingCostAsync(Address address, string shippingMethod, CancellationToken cancellationToken);
    }
}
