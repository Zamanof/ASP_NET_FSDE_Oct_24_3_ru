using ASP_NET_23._TaskFlow_CQRS.Application.DTOs;
using ASP_NET_23._TaskFlow_CQRS.Application.Interfaces;

namespace ASP_NET_23._TaskFlow_CQRS.Application.Services;

public class AuthService : IAuthService
{
    private readonly IAuthUserStore _authUserStore;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public AuthService(
        IAuthUserStore authUserStore,
        IJwtTokenService jwtTokenService,
        IRefreshTokenRepository refreshTokenRepository)
    {
        _authUserStore = authUserStore;
        _jwtTokenService = jwtTokenService;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto loginRequest)
    {
        var userId = await _authUserStore.FindUserIdByEmailOrIdAsync(loginRequest.Email);
        if (userId is null)
            throw new UnauthorizedAccessException("Invalid email or password.");

        if (!await _authUserStore.CheckPasswordAsync(userId, loginRequest.Password))
            throw new UnauthorizedAccessException("Invalid email or password.");

        return await GenerateTokensAsync(userId, loginRequest.Email);
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto registerRequest)
    {
        if (await _authUserStore.FindUserIdByEmailOrIdAsync(registerRequest.Email) is not null)
            throw new InvalidOperationException("User with this email already exists.");

        var userId = await _authUserStore.CreateUserAsync(registerRequest);
        await _authUserStore.AddToRoleAsync(userId, "User");

        return await GenerateTokensAsync(userId, registerRequest.Email);
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequest refreshTokenRequest)
    {
        var (userId, jti) = _jwtTokenService.ValidateRefreshTokenAndGetJti(refreshTokenRequest.RefreshToken);

        var storedToken = await _refreshTokenRepository.GetByJwtIdAsync(jti);
        if (storedToken is null)
            throw new UnauthorizedAccessException("Invalid refresh token");
        if (!storedToken.IsActive)
            throw new UnauthorizedAccessException("Refresh token has been revoked or expired");

        storedToken.RevokedAt = DateTime.UtcNow;

        var email = await _authUserStore.GetEmailAsync(userId);
        var newTokens = await GenerateTokensAsync(userId, email);
        var newJti = _jwtTokenService.GetJtiFromRefreshToken(newTokens.RefreshToken);
        var newStoredToken = string.IsNullOrEmpty(newJti) ? null : await _refreshTokenRepository.GetByJwtIdAsync(newJti);
        if (newStoredToken is not null)
            storedToken.ReplacedByJwtId = newStoredToken.JwtId;

        await _refreshTokenRepository.UpdateAsync(storedToken);
        return newTokens;
    }

    public async Task RevokeRefreshTokenAsync(RefreshTokenRequest refreshTokenRequest)
    {
        string jti;
        try
        {
            (_, jti) = _jwtTokenService.ValidateRefreshTokenAndGetJti(refreshTokenRequest.RefreshToken, validateLifetime: false);
        }
        catch
        {
            return;
        }

        var storedToken = await _refreshTokenRepository.GetByJwtIdAsync(jti);
        if (storedToken is null || !storedToken.IsActive) return;

        storedToken.RevokedAt = DateTime.UtcNow;
        await _refreshTokenRepository.UpdateAsync(storedToken);
    }

    private async Task<AuthResponseDto> GenerateTokensAsync(string userId, string? email)
    {
        var roles = await _authUserStore.GetRolesAsync(userId);
        var (accessToken, expiresAt) = await _jwtTokenService.GenerateAccessTokenAsync(userId, email ?? "", roles);
        var (refreshEntity, refreshJwt) = await _jwtTokenService.CreateRefreshTokenAsync(userId);

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            ExpiresAt = expiresAt,
            RefreshToken = refreshJwt,
            RefreshTokenExpiresAt = refreshEntity.ExpiresAt,
            Email = email ?? "",
            Roles = roles
        };
    }
}
