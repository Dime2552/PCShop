using MediatR;
using PCShop.Application.Auth.DTOs;

namespace PCShop.Application.Auth.Commands
{
    public record LoginCommand(string Email, string Password, string? SessionId) : IRequest<AuthResponseDto>;
}
