using Microsoft.EntityFrameworkCore;
using ASP_NET_23._TaskFlow_CQRS.Application.DTOs;
using ASP_NET_23._TaskFlow_CQRS.Application.Interfaces;
using ASP_NET_23._TaskFlow_CQRS.Domain;
using ASP_NET_23._TaskFlow_CQRS.Infrastructure.Data;
using ASP_NET_23._TaskFlow_CQRS.Infrastructure.Identity;

namespace ASP_NET_23._TaskFlow_CQRS.Infrastructure.Repositories;

public class ProjectMemberRepository : IProjectMemberRepository
{
    private readonly TaskFlowDbContext _context;

    public ProjectMemberRepository(TaskFlowDbContext context) => _context = context;

    public async Task AddAsync(ProjectMember member)
    {
        _context.ProjectMembers.Add(member);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int projectId, string userId) =>
        await _context.ProjectMembers.AnyAsync(m => m.ProjectId == projectId && m.UserId == userId);

    public async Task<ProjectMember?> FindAsync(int projectId, string userId) =>
        await _context.ProjectMembers.FirstOrDefaultAsync(m => m.ProjectId == projectId && m.UserId == userId);

    public async Task<IEnumerable<string>> GetMemberUserIdsAsync(int projectId) =>
        await _context.ProjectMembers.Where(m => m.ProjectId == projectId).Select(m => m.UserId).ToListAsync();

    public async Task<IEnumerable<ProjectMemberResponse>> GetByProjectIdWithUserAsync(int projectId)
    {
        var members = await _context.ProjectMembers
            .Where(m => m.ProjectId == projectId)
            .OrderBy(m => m.CreatedAt)
            .ToListAsync();

        var userIds = members.Select(m => m.UserId).Distinct().ToList();
        var users = await _context.Users
            .Where(u => userIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id);

        return members.Select(m =>
        {
            var user = users.GetValueOrDefault(m.UserId);
            return new ProjectMemberResponse
            {
                UserId = m.UserId,
                Email = user?.Email ?? "",
                FirstName = user?.FirstName,
                LastName = user?.LastName,
                JoinedAt = m.CreatedAt
            };
        });
    }

    public async Task RemoveAsync(ProjectMember member)
    {
        _context.ProjectMembers.Remove(member);
        await _context.SaveChangesAsync();
    }
}
