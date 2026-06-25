using MediatR;
using PCShop.Application.Auth.DTOs;

namespace PCShop.Application.Auth.Commands
{
    public record RegisterCommand(string Email, string Password, string FirstName, string LastName) : IRequest<AuthResponseDto>;
}
