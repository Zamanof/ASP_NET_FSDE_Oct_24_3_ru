using ASP_NET_21._TaskFlow.BLL.DTOs;

namespace ASP_NET_21._TaskFlow.BLL.Services;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto registerRequest);
    Task<AuthResponseDto> LoginAsync(LoginRequestDto loginRequest);
    Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequest refreshTokenRequest);
    Task RevokeRefreshTokenAsync(RefreshTokenRequest refreshTokenRequest);
}
