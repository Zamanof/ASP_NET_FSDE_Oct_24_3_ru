using ASP_NET_15._TaskFlow_Refresh_Token.DTOs.Auth_DTOs;

namespace ASP_NET_15._TaskFlow_Refresh_Token.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterRequest registerRequest);
    Task<AuthResponseDto> LoginAsync(LoginRequest loginRequest);
    Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequest refreshTokenRequest);
    Task RevokeRefreshTokenAsync(string refreshToken);
}
