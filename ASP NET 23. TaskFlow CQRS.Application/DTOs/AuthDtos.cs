namespace ASP_NET_23._TaskFlow_CQRS.Application.DTOs;

public class RegisterRequestDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class LoginRequestDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class AuthResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; set; }
    public string RefreshToken { get; set; } = string.Empty;
    public DateTimeOffset RefreshTokenExpiresAt { get; set; }
    public string Email { get; set; } = string.Empty;
    public IEnumerable<string> Roles { get; set; } = new List<string>();
}

public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}
