using TaskFlow.Application.DTOs;

namespace TaskFlow.Application.Interfaces;

public interface IAuthUserStore
{
    Task<string?> FindUserIdByEmailOrIdAsync(string emailOrId);
    Task<string?> GetEmailAsync(string userId);
    Task<bool> CheckPasswordAsync(string userId, string password);
    Task<IList<string>> GetRolesAsync(string userId);
    Task<string> CreateUserAsync(RegisterRequestDto request);
    Task AddToRoleAsync(string userId, string role);
}
