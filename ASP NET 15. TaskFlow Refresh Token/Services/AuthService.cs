using ASP_NET_15._TaskFlow_Refresh_Token.Data;
using ASP_NET_15._TaskFlow_Refresh_Token.DTOs.Auth_DTOs;
using ASP_NET_15._TaskFlow_Refresh_Token.Models;
using ASP_NET_15._TaskFlow_Refresh_Token.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace ASP_NET_15._TaskFlow_Refresh_Token.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly TaskFlowDbContext _context;

    private const string RefreshTokenType = "refresh";
    public AuthService(UserManager<ApplicationUser> userManager, IConfiguration configuration, TaskFlowDbContext context)
    {
        _userManager = userManager;
        _configuration = configuration;
        _context = context;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequest loginRequest)
    {
        var user = await _userManager.FindByEmailAsync(loginRequest.Email);

        if (user is null)
        {
            throw new UnauthorizedAccessException("Invalid name or password");
        }

        var isValidPassword = await _userManager.CheckPasswordAsync(user, loginRequest.Password);

        if (!isValidPassword)
        {
            throw new UnauthorizedAccessException("Invalid name or password");
        }
        return await GenerateTokenAsync(user);

    }


    public async Task<AuthResponseDto> RegisterAsync(RegisterRequest registerRequest)
    {
        var existingUser = await _userManager.FindByEmailAsync(registerRequest.Email);

        if (existingUser is not null)
        {
            throw new InvalidOperationException("User with this email already exists");
        }

        var user = new ApplicationUser
        {
            UserName = registerRequest.Email,
            Email = registerRequest.Email,
            FirstName = registerRequest.FirstName,
            LastName = registerRequest.LastName,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null
        };

        //await _userManager.AddToRoleAsync(user, "U")

        var result = await _userManager.CreateAsync(user, registerRequest.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(",", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"User creation failed: {errors}");
        }
        return await GenerateTokenAsync(user);
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequest refreshTokenRequest)
    {
        var (principal, jti) = ValidateRefreshJwtAndGetJti(refreshTokenRequest.RefreshToken);

        var storedToken = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.JwtId == jti);

        if(storedToken is null)
            throw new UnauthorizedAccessException("Invalid refresh token");

        if(!storedToken.IsActive)
            throw new UnauthorizedAccessException("Refresh token has been revoked or expired");

        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);

        var user = await _userManager.FindByIdAsync(userId!);

        if(user is null)
            throw new UnauthorizedAccessException("User not found");

        storedToken.RevokedAt = DateTime.UtcNow;

        var newToken = await GenerateTokenAsync(user);
        var newStoredToken = await _context
                                    .RefreshTokens
                                    .FirstOrDefaultAsync(rt => rt.JwtId == GetJtiFromRefreshToken(newToken.RefreshToken));

        if (newStoredToken is not null)
            storedToken.ReplacedByJwtId = newStoredToken.JwtId;

        await _context.SaveChangesAsync();

        return newToken;

    }
    public async Task RevokeRefreshTokenAsync(RefreshTokenRequest refreshTokenRequest)
    {
        string? jti;
        try
        {
            (_, jti) = ValidateRefreshJwtAndGetJti(refreshTokenRequest.RefreshToken, validateLifeTime: false);
        }
        catch 
        {
            return;
        }

        var storedToken = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.JwtId == jti);

        if (storedToken is null || !storedToken.IsActive) return;

        storedToken.RevokedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    private async Task<(RefreshToken entity, string jwt)>
        CreateRefreshTokenJwtAsync(string userId, int expirationDay)
    {
        var jwtSettings = _configuration.GetSection("JWTSettings");
        var refreshTokenSecretKey = jwtSettings["RefreshTokenSecretKey"];
        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];

        var expiresAt = DateTime.UtcNow.AddDays(expirationDay);
        var jti = Guid.NewGuid().ToString("N");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(refreshTokenSecretKey!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(JwtRegisteredClaimNames.Jti, jti),
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim("token_type", RefreshTokenType),
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires:expiresAt,
            signingCredentials: credentials            
            );
        var jwtString = new JwtSecurityTokenHandler().WriteToken(token);
        var entity = new RefreshToken
        {
            JwtId = jti,
            UserId = userId,
            ExpiresAt = expiresAt,
            CreatedAt = DateTime.UtcNow
        };

        _context.RefreshTokens.Add(entity);

        await _context.SaveChangesAsync();

        return (entity, jwtString);
    }

    private (ClaimsPrincipal principal, string jti)
        ValidateRefreshJwtAndGetJti(string refreshToken, bool validateLifeTime = true)
    {
        var jwtSettings = _configuration.GetSection("JWTSettings");
        var refreshTokenSecretKey = jwtSettings["RefreshTokenSecretKey"];
        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];

        var handler = new JwtSecurityTokenHandler();

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(refreshTokenSecretKey!));

        var principal = handler.ValidateToken(refreshToken, new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateLifetime = validateLifeTime,
            ClockSkew = TimeSpan.Zero
        }, out var validatedToken);

        if (validatedToken is not JwtSecurityToken jwt)
            throw new UnauthorizedAccessException("Invalid refresh token");

        var tokenType = jwt.Claims.FirstOrDefault(c => c.Type == "token_type")?.Value;

        if(tokenType != RefreshTokenType)
            throw new UnauthorizedAccessException("Invalid refresh token");

        var jti = jwt.Claims.FirstOrDefault(c=> c.Type == JwtRegisteredClaimNames.Jti)?.Value
            ?? throw new UnauthorizedAccessException("Invalid refresh token");

        return (principal, jti);
    }

    private static string GetJtiFromRefreshToken(string refreshJwt)
    {
        var handler = new JwtSecurityTokenHandler();

        if (!handler.CanReadToken(refreshJwt)) return string.Empty;

        var jwt = handler.ReadJwtToken(refreshJwt);

        return jwt.Claims
            .FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value
            ?? string.Empty;
    }

    private async Task<AuthResponseDto> GenerateTokenAsync(ApplicationUser user)
    {
        var jwtSettings = _configuration.GetSection("JWTSettings");
        var secretKey = jwtSettings["SecretKey"];
        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];
        var expirationInMinutes = 
                int.Parse(jwtSettings["ExpirationInMinutes"]!);
        var refreshTokenExpirationInDays = 
                int.Parse(jwtSettings["RefreshTokenExpirationInDays"]!);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name , user.UserName!),
            new Claim(ClaimTypes.Email , user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti , Guid.NewGuid().ToString())
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationInMinutes),
            signingCredentials: credentials
            );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        var (refreshToken, refreshJwt) = 
                await CreateRefreshTokenJwtAsync(user.Id, refreshTokenExpirationInDays);
        return new AuthResponseDto
        {
            Email = user.Email!,
            AccessToken = tokenString,
            ExpiredAt = DateTime.UtcNow.AddMinutes(expirationInMinutes),
            RefreshToken = refreshJwt,
            RefreshTokenExpiredAt = refreshToken.ExpiresAt,
            Roles = roles
        };
    }
}
