using Microsoft.EntityFrameworkCore;
using ASP_NET_23._TaskFlow_CQRS.Application.DTOs;
using ASP_NET_23._TaskFlow_CQRS.Application.Interfaces;
using ASP_NET_23._TaskFlow_CQRS.Infrastructure.Data;
using ASP_NET_23._TaskFlow_CQRS.Infrastructure.Identity;

namespace ASP_NET_23._TaskFlow_CQRS.Infrastructure.Repositories;

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
