using ASP_NET_13._TaskFLow_Authentication_and_Authorization.DTOs.Auth_DTOs;
using ASP_NET_13._TaskFLow_Authentication_and_Authorization.Models;
using ASP_NET_13._TaskFLow_Authentication_and_Authorization.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace ASP_NET_13._TaskFLow_Authentication_and_Authorization.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequest loginRequest)
    {
        var user = await _userManager.FindByEmailAsync(loginRequest.Email);

        if(user is null)
        {
            throw new UnauthorizedAccessException("Invalid name or password");
        }

        var isValidPassword = await _userManager.CheckPasswordAsync(user, loginRequest.Password);

        if (!isValidPassword)
        {
            throw new UnauthorizedAccessException("Invalid name or password");
        }
        return new AuthResponseDto
        {
            Email = user.Email!
        };

    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequest registerRequest)
    {
        var existingUser = await _userManager.FindByEmailAsync(registerRequest.Email);

        if(existingUser is not null)
        {
            throw new InvalidOperationException("User with this email already exists");
        }

        var user = new ApplicationUser
        {
            UserName = registerRequest.Email,
            Email = registerRequest.Email,
            FirstName = registerRequest.FirstName,
            LastName = registerRequest.LastName,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null
        };

        var result = await _userManager.CreateAsync(user, registerRequest.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(",", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"User creation failed: {errors}");
        }
        return new AuthResponseDto
        {
            Email = user.Email
        };
    }
}
