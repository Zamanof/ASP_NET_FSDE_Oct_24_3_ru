using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ASP_NET_23._TaskFlow_CQRS.Application.Config;
using ASP_NET_23._TaskFlow_CQRS.Application.Interfaces;
using ASP_NET_23._TaskFlow_CQRS.Domain;

namespace ASP_NET_23._TaskFlow_CQRS.Infrastructure.Jwt;

public class JwtTokenService : IJwtTokenService
{
    private const string RefreshTokenType = "refresh";
    private readonly JwtConfig _config;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public JwtTokenService(IOptions<JwtConfig> config, IRefreshTokenRepository refreshTokenRepository)
    {
        _config = config.Value ?? throw new InvalidOperationException("JwtConfig is not configured.");
        if (string.IsNullOrWhiteSpace(_config.SecretKey))
            throw new InvalidOperationException("JwtConfig.SecretKey is not set. Check appsettings.json section JWTSettings.");
        if (string.IsNullOrWhiteSpace(_config.RefreshTokenSecretKey))
            throw new InvalidOperationException("JwtConfig.RefreshTokenSecretKey is not set. Check appsettings.json section JWTSettings.");
        _refreshTokenRepository = refreshTokenRepository;
    }

    public Task<(string AccessToken, DateTimeOffset ExpiresAt)> GenerateAccessTokenAsync(string userId, string email, IList<string> roles)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId),
            new(ClaimTypes.Name, email),
            new(ClaimTypes.Email, email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var expiresAt = DateTime.UtcNow.AddMinutes(_config.ExpirationInMinutes);
        var token = new JwtSecurityToken(
            issuer: _config.Issuer,
            audience: _config.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        return Task.FromResult((tokenString, (DateTimeOffset)expiresAt));
    }

    public async Task<(RefreshToken Entity, string Jwt)> CreateRefreshTokenAsync(string userId)
    {
        var jti = Guid.NewGuid().ToString("N");
        var expiresAt = DateTime.UtcNow.AddDays(_config.RefreshTokenExpirationInDays);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.RefreshTokenSecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId),
            new(JwtRegisteredClaimNames.Jti, jti),
            new(JwtRegisteredClaimNames.Sub, userId),
            new("token_type", RefreshTokenType)
        };

        var token = new JwtSecurityToken(
            issuer: _config.Issuer,
            audience: _config.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        var jwtString = new JwtSecurityTokenHandler().WriteToken(token);

        var entity = new RefreshToken
        {
            JwtId = jti,
            UserId = userId,
            ExpiresAt = expiresAt,
            CreatedAt = DateTime.UtcNow
        };
        await _refreshTokenRepository.AddAsync(entity);
        return (entity, jwtString);
    }

    public (string UserId, string Jti) ValidateRefreshTokenAndGetJti(string refreshJwt, bool validateLifetime = true)
    {
        var handler = new JwtSecurityTokenHandler();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.RefreshTokenSecretKey));

        var principal = handler.ValidateToken(refreshJwt, new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateIssuer = true,
            ValidIssuer = _config.Issuer,
            ValidateAudience = true,
            ValidAudience = _config.Audience,
            ValidateLifetime = validateLifetime,
            ClockSkew = TimeSpan.Zero
        }, out var validatedToken);

        if (validatedToken is not JwtSecurityToken jwt)
            throw new UnauthorizedAccessException("Invalid refresh token");

        if (jwt.Claims.FirstOrDefault(c => c.Type == "token_type")?.Value != RefreshTokenType)
            throw new UnauthorizedAccessException("Invalid refresh token");

        var jti = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value
            ?? throw new UnauthorizedAccessException("Invalid refresh token");

        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("Invalid refresh token");

        return (userId, jti);
    }

    public string GetJtiFromRefreshToken(string refreshJwt)
    {
        var handler = new JwtSecurityTokenHandler();
        if (!handler.CanReadToken(refreshJwt)) return string.Empty;
        var jwt = handler.ReadJwtToken(refreshJwt);
        return jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value ?? string.Empty;
    }
}
