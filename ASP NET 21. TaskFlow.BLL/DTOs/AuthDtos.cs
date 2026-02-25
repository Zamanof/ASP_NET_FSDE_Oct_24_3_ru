namespace ASP_NET_21._TaskFlow.BLL.DTOs;

public class RegisterRequestDto
{
    /// <summary>
    /// User Name
    /// </summary>
    /// <example>John</example>
    public string FirstName { get; set; } = string.Empty;
    /// <summary>
    /// User LastName
    /// </summary>
    /// <example>Doe</example>
    public string LastName { get; set; } = string.Empty;
    /// <summary>
    /// User Email
    /// </summary>
    /// <example>john@doe.com</example>
    public string Email { get; set; } = string.Empty;
    /// <summary>
    /// Password
    /// </summary>
    /// <example>P@ssword123!</example>
    public string Password { get; set; } = string.Empty;
    /// <summary>
    /// Confirmed Password
    /// </summary>
    /// <example>P@ssword123!</example>
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class LoginRequestDto
{
    /// <summary>
    /// User Email
    /// </summary>
    /// <example>john@doe.com</example>
    public string Email { get; set; } = string.Empty;
    /// <summary>
    /// Password
    /// </summary>
    /// <example>P@ssword123!</example>
    public string Password { get; set; } = string.Empty;
}

public class AuthResponseDto
{
    /// <summary>
    /// Access token
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;
    /// <summary>
    /// Token Expires date 
    /// </summary>
    public DateTimeOffset ExpiresAt { get; set; }
    /// <summary>
    /// Refresh token
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;
    /// <summary>
    /// Token Expires date 
    /// </summary>
    public DateTimeOffset RefreshTokenExpiresAt { get; set; }

    /// <summary>
    /// User Email
    /// </summary>
    /// <example>john@doe.com</example>
    public string Email { get; set; } = string.Empty;
    /// <summary>
    /// User Roles
    /// </summary>
    public IEnumerable<string> Roles { get; set; } = new List<string>();
}

public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}