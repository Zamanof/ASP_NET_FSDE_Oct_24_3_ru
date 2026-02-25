using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.DTOs;
using TaskFlow.Application.Interfaces;
using TaskFlow.Infrastructure.Data;
using TaskFlow.Infrastructure.Identity;

namespace TaskFlow.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly TaskFlowDbContext _context;

    public UserRepository(TaskFlowDbContext context) => _context = context;

    public async Task<IEnumerable<AvailableUserDto>> GetOrderedByEmailExceptIdsAsync(IEnumerable<string> excludeIds)
    {
        var ids = excludeIds.ToList();
        return await _context.Users
            .Where(u => !ids.Contains(u.Id))
            .OrderBy(u => u.Email)
            .Select(u => new AvailableUserDto
            {
                Id = u.Id,
                Email = u.Email!,
                FirstName = u.FirstName,
                LastName = u.LastName
            })
            .ToListAsync();
    }
}
