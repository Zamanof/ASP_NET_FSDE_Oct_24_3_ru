using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Interfaces;
using TaskFlow.Domain;
using TaskFlow.Infrastructure.Data;

namespace TaskFlow.Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly TaskFlowDbContext _context;

    public RefreshTokenRepository(TaskFlowDbContext context) => _context = context;

    public async Task<RefreshToken> AddAsync(RefreshToken refreshToken)
    {
        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();
        return refreshToken;
    }

    public async Task<RefreshToken?> GetByJwtIdAsync(string jwtId) =>
        await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.JwtId == jwtId);

    public async Task UpdateAsync(RefreshToken refreshToken)
    {
        _context.RefreshTokens.Update(refreshToken);
        await _context.SaveChangesAsync();
    }
}
