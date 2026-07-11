using PCShop.Domain.Entities;

namespace PCShop.Application.Common.Interfaces
{
    public interface IPaymentService
    {
        Task<(string SessionId, string CheckoutUrl)> CreateCheckoutSessionAsync(Order order, string successUrl, string cancelUrl, CancellationToken cancellationToken);
    }
}
