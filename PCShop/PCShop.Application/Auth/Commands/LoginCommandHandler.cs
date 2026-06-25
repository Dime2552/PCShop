using MediatR;
using Microsoft.EntityFrameworkCore;
using PCShop.Application.Auth.DTOs;
using PCShop.Application.Common.Exceptions;
using PCShop.Application.Common.Interfaces;

namespace PCShop.Application.Auth.Commands
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
    {
        private readonly IAppDbContext _context;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtProvider _jwtProvider;
        private readonly ICartService _cartService;

        public LoginCommandHandler(IAppDbContext context, IPasswordHasher passwordHasher, IJwtProvider jwtProvider, ICartService cartService)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _jwtProvider = jwtProvider;
            _cartService = cartService;
        }

        public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

            if (user == null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
                throw new BadRequestException("Invalid email or password.");

            // Cart Merge Logic
            if (!string.IsNullOrEmpty(request.SessionId))
            {
                var sessionCartId = $"cart:session:{request.SessionId}";
                var userCartId = $"cart:user:{user.Id}";

                var sessionCart = await _cartService.GetCartAsync(sessionCartId);
                if (sessionCart.Any())
                {
                    var userCart = await _cartService.GetCartAsync(userCartId);

                    // Merge items: add new ones or update quantities for existing
                    foreach (var item in sessionCart)
                    {
                        var existingItem = userCart.FirstOrDefault(x => x.ProductId == item.ProductId);
                        if (existingItem != null)
                        {
                            existingItem.Quantity += item.Quantity;
                        }
                        else
                        {
                            userCart.Add(item);
                        }
                    }

                    // Save merged cart and remove session cart
                    await _cartService.SetCartAsync(userCartId, userCart);
                    await _cartService.DeleteCartAsync(sessionCartId);
                }
            }

            var token = _jwtProvider.GenerateToken(user);

            return new AuthResponseDto(token, user.Email, user.FirstName, user.Role.ToString());
        }
    }
}
