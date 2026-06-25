namespace PCShop.Application.Auth.DTOs
{
    public record AuthResponseDto(string Token, string Email, string FirstName, string Role);
}
