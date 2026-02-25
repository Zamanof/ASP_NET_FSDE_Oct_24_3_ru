using ASP_NET_23._TaskFlow_CQRS.Application.DTOs;

namespace ASP_NET_23._TaskFlow_CQRS.Application.Services;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto registerRequest);
    Task<AuthResponseDto> LoginAsync(LoginRequestDto loginRequest);
    Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequest refreshTokenRequest);
    Task RevokeRefreshTokenAsync(RefreshTokenRequest refreshTokenRequest);
}
