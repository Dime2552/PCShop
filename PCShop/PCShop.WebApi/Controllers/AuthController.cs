using MediatR;
using Microsoft.AspNetCore.Mvc;
using PCShop.Application.Auth.Commands;
using PCShop.Application.Auth.DTOs;

namespace PCShop.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginRequest request)
        {
            // Extract session id from headers to merge cart
            Request.Headers.TryGetValue("x-session-id", out var sessionId);

            var result = await _mediator.Send(new LoginCommand(request.Email, request.Password, sessionId));
            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register(RegisterRequest request)
        {
            var result = await _mediator.Send(new RegisterCommand(request.Email, request.Password, request.FirstName, request.LastName));
            return Ok(result);
        }
    }

    public record LoginRequest(string Email, string Password);
    public record RegisterRequest(string Email, string Password, string FirstName, string LastName);
}
