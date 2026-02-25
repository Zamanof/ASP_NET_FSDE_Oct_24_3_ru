using ASP_NET_21._TaskFlow.BLL.Common;
using ASP_NET_21._TaskFlow.BLL.DTOs;
using ASP_NET_21._TaskFlow.BLL.Services;
using Microsoft.AspNetCore.Mvc;

namespace ASP_NET_21._TaskFlow.API.Controllers;

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
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Register(
        [FromBody] RegisterRequestDto registerRequest)
    {
        var result = await _authService.RegisterAsync(registerRequest);
        return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result, "User registered successfully"));
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login(
        [FromBody] LoginRequestDto loginRequest)
    {
        var result = await _authService.LoginAsync(loginRequest);
        return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result, "Login successfully"));

    }

    [HttpPost("refresh")]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Refresh(
        [FromBody] RefreshTokenRequest refreshTokenRequest)
    {
        var result = await _authService.RefreshTokenAsync(refreshTokenRequest);
        return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result, "Token refresh successfully"));
    }

    [HttpPost("revoke")]
    public async Task<ActionResult> Revoke(
        [FromBody] RefreshTokenRequest refreshTokenRequest)
    {
        await _authService.RevokeRefreshTokenAsync(refreshTokenRequest);
        return Ok(ApiResponse<object>.SuccessResponse("Token revoke successfully"));
    }
}
