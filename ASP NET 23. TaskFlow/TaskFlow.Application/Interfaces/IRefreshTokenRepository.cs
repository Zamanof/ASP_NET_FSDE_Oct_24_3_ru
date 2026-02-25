using TaskFlow.Domain;

namespace TaskFlow.Application.Interfaces;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByJwtIdAsync(string jwtId);
    Task<RefreshToken> AddAsync(RefreshToken refreshToken);
    Task UpdateAsync(RefreshToken refreshToken);
}
