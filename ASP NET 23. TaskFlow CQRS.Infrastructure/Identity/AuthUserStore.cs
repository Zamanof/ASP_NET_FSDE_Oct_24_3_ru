using Microsoft.AspNetCore.Identity;
using ASP_NET_23._TaskFlow_CQRS.Application.DTOs;
using ASP_NET_23._TaskFlow_CQRS.Application.Interfaces;

namespace ASP_NET_23._TaskFlow_CQRS.Infrastructure.Identity;

public class AuthUserStore : IAuthUserStore
{
    private readonly UserManager<AppUser> _userManager;

    public AuthUserStore(UserManager<AppUser> userManager) => _userManager = userManager;

    public async Task<string?> FindUserIdByEmailOrIdAsync(string emailOrId)
    {
        var user = emailOrId.Contains('@')
            ? await _userManager.FindByEmailAsync(emailOrId)
            : await _userManager.FindByIdAsync(emailOrId);
        return user?.Id;
    }

    public async Task<string?> GetEmailAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user?.Email;
    }

    public async Task<bool> CheckPasswordAsync(string userId, string password)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user is not null && await _userManager.CheckPasswordAsync(user, password);
    }

    public async Task<IList<string>> GetRolesAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user is null ? new List<string>() : await _userManager.GetRolesAsync(user);
    }

    public async Task<string> CreateUserAsync(RegisterRequestDto request)
    {
        var user = new AppUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null
        };
        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            throw new InvalidOperationException($"User creation failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        return user.Id;
    }

    public async Task AddToRoleAsync(string userId, string role) =>
        await _userManager.AddToRoleAsync((await _userManager.FindByIdAsync(userId))!, role);
}
