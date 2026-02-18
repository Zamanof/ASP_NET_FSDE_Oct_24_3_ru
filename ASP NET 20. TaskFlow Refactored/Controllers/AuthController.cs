using ASP_NET_20._TaskFlow_Refactored.Common;
using ASP_NET_20._TaskFlow_Refactored.DTOs;
using ASP_NET_20._TaskFlow_Refactored.Services;
using Microsoft.AspNetCore.Mvc;

namespace ASP_NET_20._TaskFlow_Refactored.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }
    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Register([FromBody] RegisterRequest registerRequest)
    {
        var result = await _authService.RegisterAsync(registerRequest);
        return Ok(
             ApiResponse<AuthResponseDto>
                 .SuccessResponse(result, "User registered successfully")
         );
    }
    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login([FromBody] LoginRequest loginRequest)
    {
        var result = await _authService.LoginAsync(loginRequest);
        return Ok(
             ApiResponse<AuthResponseDto>
                 .SuccessResponse(result, "Login successfully")
         );
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> 
        Refresh([FromBody] RefreshTokenRequest refreshTokenRequest)
    {
        var result = await _authService.RefreshTokenAsync(refreshTokenRequest);
        return Ok(
             ApiResponse<AuthResponseDto>
                 .SuccessResponse(result, "Refresh access token successfully")
         );
    }

    [HttpPost("revoke")]
    public async Task<ActionResult>
        Revoke([FromBody] RefreshTokenRequest refreshTokenRequest)
    {
        await _authService.RevokeRefreshTokenAsync(refreshTokenRequest);
        return Ok(
             ApiResponse<AuthResponseDto>
                 .SuccessResponse("Token revoked successfully")
         );
    }
}
