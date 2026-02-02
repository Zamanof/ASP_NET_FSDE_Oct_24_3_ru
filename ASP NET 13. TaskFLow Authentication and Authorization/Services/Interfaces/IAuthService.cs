using ASP_NET_13._TaskFLow_Authentication_and_Authorization.DTOs.Auth_DTOs;

namespace ASP_NET_13._TaskFLow_Authentication_and_Authorization.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterRequest registerRequest);
    Task<AuthResponseDto> LoginAsync(LoginRequest loginRequest);
}
