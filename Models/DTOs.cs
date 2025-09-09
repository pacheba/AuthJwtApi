namespace AuthJwtApi.Models
{
    public record RegisterRequest(string Username, string Email, string Password);
    public record LoginRequest(string Username, string Password);
    public record AuthResponse(string Token, int ExpiresInMinutes);
    public record UserResponse(int Id, string Username, string Email);
}
