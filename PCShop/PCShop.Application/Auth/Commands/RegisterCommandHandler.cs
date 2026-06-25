using MediatR;
using Microsoft.EntityFrameworkCore;
using PCShop.Application.Auth.DTOs;
using PCShop.Application.Common.Exceptions;
using PCShop.Application.Common.Interfaces;
using PCShop.Domain.Entities;
using PCShop.Domain.Enums;

namespace PCShop.Application.Auth.Commands
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponseDto>
    {
        private readonly IAppDbContext _context;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtProvider _jwtProvider;

        public RegisterCommandHandler(IAppDbContext context, IPasswordHasher passwordHasher, IJwtProvider jwtProvider)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _jwtProvider = jwtProvider;
        }

        public async Task<AuthResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            // Check if user exists
            var existingUser = await _context.Users.AnyAsync(u => u.Email == request.Email, cancellationToken);
            if (existingUser)
                throw new BadRequestException("Email is already in use.");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                PasswordHash = _passwordHasher.Hash(request.Password),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Role = UserRole.Customer // Default role
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync(cancellationToken);

            var token = _jwtProvider.GenerateToken(user);

            return new AuthResponseDto(token, user.Email, user.FirstName, user.Role.ToString());
        }
    }
}
