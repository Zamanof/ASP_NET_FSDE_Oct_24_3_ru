using ASP_NET_21._TaskFlow.Data;
using ASP_NET_21._TaskFlow.Models;
using Microsoft.EntityFrameworkCore;

namespace ASP_NET_21._TaskFlow.DAL;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly TaskFlowDbContext _context;

    public RefreshTokenRepository(TaskFlowDbContext context)
    {
        _context = context;
    }

    public async Task<RefreshToken> AddAsync(RefreshToken refreshToken)
    {
        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();
        return refreshToken;
    }

    public async Task<RefreshToken?> GetByJwtIdAsync(string jwtId)
    {
        return await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.JwtId == jwtId);
    }

    public async Task UpdateAsync(RefreshToken refreshToken)
    {
        _context.RefreshTokens.Update(refreshToken);
        await _context.SaveChangesAsync();
    }
}
