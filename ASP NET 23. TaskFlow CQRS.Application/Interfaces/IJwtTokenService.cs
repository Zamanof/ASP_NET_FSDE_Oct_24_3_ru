using ASP_NET_23._TaskFlow_CQRS.Application.DTOs;
using ASP_NET_23._TaskFlow_CQRS.Domain;

namespace ASP_NET_23._TaskFlow_CQRS.Application.Interfaces;

public interface IJwtTokenService
{
    Task<(string AccessToken, DateTimeOffset ExpiresAt)> GenerateAccessTokenAsync(string userId, string email, IList<string> roles);
    Task<(RefreshToken Entity, string Jwt)> CreateRefreshTokenAsync(string userId);
    (string UserId, string Jti) ValidateRefreshTokenAndGetJti(string refreshJwt, bool validateLifetime = true);
    string GetJtiFromRefreshToken(string refreshJwt);
}
