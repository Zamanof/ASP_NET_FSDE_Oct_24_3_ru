using ASP_NET_13._TaskFLow_Authentication_and_Authorization.Common;
using ASP_NET_13._TaskFLow_Authentication_and_Authorization.DTOs.Auth_DTOs;
using ASP_NET_13._TaskFLow_Authentication_and_Authorization.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ASP_NET_13._TaskFLow_Authentication_and_Authorization.Controllers;

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
}
