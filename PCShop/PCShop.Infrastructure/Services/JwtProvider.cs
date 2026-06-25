using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PCShop.Application.Common.Interfaces;
using PCShop.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PCShop.Infrastructure.Services
{
    public class JwtProvider : IJwtProvider
    {
        private readonly IConfiguration _configuration;

        public JwtProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(User user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var secretKey = jwtSettings["Key"]!;
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];
            var expireDays = int.Parse(jwtSettings["ExpireDays"]!);

            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("FirstName", user.FirstName),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer,
                audience,
                claims,
                null,
                DateTime.UtcNow.AddDays(expireDays),
                credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
