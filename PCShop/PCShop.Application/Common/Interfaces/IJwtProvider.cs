using PCShop.Domain.Entities;

namespace PCShop.Application.Common.Interfaces
{
    public interface IJwtProvider
    {
        string GenerateToken(User user);
    }
}
