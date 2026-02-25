using ASP_NET_21._TaskFlow.Models;

namespace ASP_NET_21._TaskFlow.DAL;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByJwtIdAsync(string jwtId);
    Task<RefreshToken> AddAsync(RefreshToken refreshToken);
    Task UpdateAsync(RefreshToken refreshToken);
}
