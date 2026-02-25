using TaskFlow.Application.DTOs;

namespace TaskFlow.Application.Services;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto registerRequest);
    Task<AuthResponseDto> LoginAsync(LoginRequestDto loginRequest);
    Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequest refreshTokenRequest);
    Task RevokeRefreshTokenAsync(RefreshTokenRequest refreshTokenRequest);
}
