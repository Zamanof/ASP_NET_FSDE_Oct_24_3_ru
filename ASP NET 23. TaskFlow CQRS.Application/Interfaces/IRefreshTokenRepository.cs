using ASP_NET_23._TaskFlow_CQRS.Domain;

namespace ASP_NET_23._TaskFlow_CQRS.Application.Interfaces;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByJwtIdAsync(string jwtId);
    Task<RefreshToken> AddAsync(RefreshToken refreshToken);
    Task UpdateAsync(RefreshToken refreshToken);
}
