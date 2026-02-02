namespace ASP_NET_13._TaskFLow_Authentication_and_Authorization.DTOs.Auth_DTOs;

public class RegisterRequest
{
    /// <summary>
    /// User FirstName
    /// </summary>
    /// <example>John</example>
    public string FirstName { get; set; } = string.Empty;
    /// <summary>
    /// User LastName
    /// </summary>
    /// <example>Doe</example>
    public string LastName { get; set; } = string.Empty;
    /// <summary>
    /// Email
    /// </summary>
    /// <example>john@doe.com</example>
    public string Email { get; set; } = string.Empty;
    /// <summary>
    /// Password
    /// </summary>
    /// <example>P@ss123</example>
    public string Password { get; set; } = string.Empty;
    /// <summary>
    /// Confirmed Password
    /// </summary>
    /// <example>P@ss123</example>
    public string ConfirmedPassword { get; set; } = string.Empty;
}

public class LoginRequest
{
    /// <summary>
    /// Email
    /// </summary>
    /// <example>john@doe.com</example>
    public string Email { get; set; } = string.Empty;
    /// <summary>
    /// Password
    /// </summary>
    /// <example>P@ss123</example>
    public string Password { get; set; } = string.Empty;
}

public class AuthResponseDto
{
    /// <summary>
    /// Email
    /// </summary>
    /// <example>john@doe.com</example>
    public string Email { get; set; } = string.Empty;
}